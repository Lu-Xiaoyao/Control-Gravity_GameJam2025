using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllControl : MonoBehaviour
{
    public class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameManager();
                return _instance;
            }
        }
        public float moveSpeed = 6f;
        public float spinSpeed = 25f;        
        public float cameraSpeed = 10f;
        public int deathCount = 0;
        public float gravityExtentMax = 2f;
        public float gravityExtentMin = 0.5f;
        public float gravityAreaShowTime = 3f;
        //public float cameraSizeNormal = 10f;
        //public float cameraSizeEnlarge = 25f;
        public int levelComplete = 0;
    }

}
