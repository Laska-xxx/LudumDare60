using System.Collections;
using UnityEngine;
using Zenject;

namespace Machines
{
    public class TicketsDispenser : MonoBehaviour
    {
        [SerializeField] private Rigidbody ticketPrefab;
        [SerializeField] private Transform startPoint;
        [SerializeField] private float feedSpeed = 0.3f;
        [SerializeField] private int maxPhysicalTickets = 100;
        [SerializeField] private float bendLimit = 70f;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip printSound;

        [Header("Hints")]
        [SerializeField] private string collectHint = "TAKE TICKETS";
        [SerializeField] private string printingHint = "PRINTING...";

        public bool HasTickets => _ribbon != null;
        public bool IsPrinting { get; private set; }
        public string RibbonHint => IsPrinting ? printingHint : collectHint;

        private SessionManager _sessionManager;
        private TicketRibbon _ribbon;
        private Rigidbody _heldTicket;
        private int _ticketsAmount;
        private float _ticketLength;
        private float _halfLengthLocal;

        [Inject]
        private void Init(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        private void Awake()
        {
            BoxCollider box = ticketPrefab.GetComponent<BoxCollider>();
            _halfLengthLocal = box.size.z * 0.5f;
            _ticketLength = box.size.z * ticketPrefab.transform.localScale.z;
        }

        public void Dispense(int amount)
        {
            if (amount <= 0 || HasTickets)
                return;

            _ticketsAmount = amount;
            _sessionManager.RegisterRibbon();
            StartCoroutine(DispenseRoutine(Mathf.Min(amount, maxPhysicalTickets)));
        }

        public void Collect()
        {
            if (!HasTickets || IsPrinting)
                return;

            Destroy(_ribbon.gameObject);
            _ribbon = null;
            _heldTicket = null;

            _sessionManager.CollectRibbon(_ticketsAmount);

            _ticketsAmount = 0;
        }

        private IEnumerator DispenseRoutine(int amount)
        {
            IsPrinting = true;

            _ribbon = new GameObject("TicketRibbon").AddComponent<TicketRibbon>();
            _ribbon.Init(this);

            for (int i = 0; i < amount; i++)
            {
                Rigidbody ticket = Instantiate(ticketPrefab, startPoint.position, startPoint.rotation, _ribbon.transform);
                ticket.isKinematic = true;

                if (audioSource != null && printSound != null)
                    audioSource.PlayOneShot(printSound);

                if (_heldTicket != null)
                {
                    ConnectTickets(_heldTicket, ticket);
                    _heldTicket.isKinematic = false;
                }

                _heldTicket = ticket;

                float moved = 0f;
                while (moved < _ticketLength)
                {
                    float step = Mathf.Min(feedSpeed * Time.fixedDeltaTime, _ticketLength - moved);
                    moved += step;
                    ticket.MovePosition(ticket.position + startPoint.forward * step);
                    yield return new WaitForFixedUpdate();
                }
            }

            IsPrinting = false;
        }

        private void ConnectTickets(Rigidbody outer, Rigidbody inner)
        {
            HingeJoint joint = outer.gameObject.AddComponent<HingeJoint>();
            joint.connectedBody = inner;
            joint.anchor = new Vector3(0f, 0f, -_halfLengthLocal);
            joint.axis = Vector3.right;
            joint.useLimits = true;
            joint.limits = new JointLimits { min = -bendLimit, max = bendLimit };
        }
    }
}