using UnityEngine;

namespace CantuniasInferno
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] int scoreForWin = 3;

        public static GameManager Instance { get; private set; }

        int score;

        public string Score => $"{score}/{scoreForWin}";

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void AddScore(int amount)
        {
            score += amount;
        }
    }
}
