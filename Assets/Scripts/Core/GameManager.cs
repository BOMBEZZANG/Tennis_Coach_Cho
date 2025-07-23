using UnityEngine;
using TennisCoachCho.Player;
using TennisCoachCho.Progression;
using TennisCoachCho.UI;

namespace TennisCoachCho.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Systems")]
        [SerializeField] private TimeSystem timeSystem;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private ProgressionManager progressionManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private AppointmentManager appointmentManager;
        
        public static GameManager Instance { get; private set; }
        
        public TimeSystem TimeSystem => timeSystem;
        public PlayerController PlayerController => playerController;
        public ProgressionManager ProgressionManager => progressionManager;
        public UIManager UIManager => uiManager;
        public AppointmentManager AppointmentManager => appointmentManager;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSystems();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeSystems()
        {
            Debug.Log("GameManager.InitializeSystems() called!");
            timeSystem?.Initialize();
            progressionManager?.Initialize();
            appointmentManager?.Initialize();
            uiManager?.Initialize();
        }
        
        private void Start()
        {
            StartGame();
        }
        
        private void StartGame()
        {
            playerController?.SpawnAtHome();
            timeSystem?.StartTime();
            uiManager?.ShowMainHUD();
        }
    }
}