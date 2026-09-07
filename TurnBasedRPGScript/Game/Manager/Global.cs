using System;
using UnityEngine;

namespace S7
{
    public partial class Global : Singleton<Global>
    {
        [SerializeField] GameObject logView;

        public CharacterData selectCharacter;

        bool _isFirst = false;

        public bool IsFirst { get => _isFirst; }

        void Start()
        {
        }

        public void Initialize()
        {
            SystemManager.Instance.Initialize();
            _isFirst = PlayerPrefs.GetInt("first", 0) == 0 ? true : false;
            PlayerPrefs.SetInt("first", 1);



            //GameFlowManager.Instance.Initialize();

#if ENABLE_LOG
            logView.SetActive(true);
#else
        logView.SetActive(false);
#endif
            InitializeCharacter();
        }             
    }
}
