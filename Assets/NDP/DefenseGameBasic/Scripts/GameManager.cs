using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.DefenseBasic
{
    public class GameManager : MonoBehaviour, IComponentChecking
    {
        public static GameManager Ins;

        public float spawnTime;
        public Enemy[] enemyPrefabs;
        public ShopManager shopMng;
        private Player m_curPlayer;
        private bool m_isGameover;
        private int m_score;
        public AudioController auCtr;

        public int Score { get => m_score; set => m_score = value; }

        private void Awake()
        {
            //Ins = this;
            //MakeSingleton();
        }

        //private void MakeSingleton()
        //{
        //    if(Ins == null)
        //    {
        //        Ins = this;
        //        DontDestroyOnLoad(this);
        //    }
        //    else
        //    {
        //        Destroy(gameObject);
        //    }
        //}

        // Start is called before the first frame update
        void Start()
        {
            
            if (IsComponentsNull()) return;

            GUIManager.Ins.ShowGameGUI(false);
            GUIManager.Ins.UpdateMainCoins();
        }

        public bool IsComponentsNull()
        {
            return GUIManager.Ins == null || shopMng == null || AudioController.Ins == null;
        }

        public void PlayGame()
        {
            if (IsComponentsNull()) return;

            ActivePlayer();

            StartCoroutine(SpawnEnemy());

            GUIManager.Ins.ShowGameGUI(true);
            GUIManager.Ins.UpdateGameplayCoins();
            AudioController.Ins.PlayBgm();
        }

        public void ActivePlayer()
        {
            if (IsComponentsNull()) return;

            if(m_curPlayer)
                Destroy(m_curPlayer.gameObject);

            var shopItems = shopMng.items;

            if (shopItems == null || shopItems.Length <= 0) return;

            var newPlayerPb = shopItems[Pref.curPlayerId].playerPrefab;

            if (newPlayerPb)
                m_curPlayer = Instantiate(newPlayerPb, new Vector3(-7f, -1f, 0f), Quaternion.identity);
        }

        public void Gameover()
        {
            if (m_isGameover) return;

            m_isGameover = true;

            Pref.bestScore = m_score;

            if(GUIManager.Ins.gameoverDialog)
                GUIManager.Ins.gameoverDialog.Show(true);

            AudioController.Ins.PlaySound(AudioController.Ins.gameover);
        }

        IEnumerator SpawnEnemy()
        {
            while (!m_isGameover)
            {
                if (enemyPrefabs != null && enemyPrefabs.Length > 0)
                {
                    int randIdx = Random.Range(0, enemyPrefabs.Length);//(0, 3) 0, 1, 2

                    Enemy enemyPrefab = enemyPrefabs[randIdx];

                    if (enemyPrefab)
                    {
                        Instantiate(enemyPrefab, new Vector3(8, 0, 0), Quaternion.identity);
                    }
                }

                yield return new WaitForSeconds(spawnTime);
            }
        }
    }
}
