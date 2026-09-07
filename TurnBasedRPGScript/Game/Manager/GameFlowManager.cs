using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;


namespace S7
{  
    public partial class GameFlowManager : Singleton<GameFlowManager>
    {
        private bool _isTransitioning;
        
        public PresentationCore PresentationCore { get; private set; }        
        
        void Start()
        {
        }

        public void Initialize()
        {
            PresentationCore = new PresentationCore();            
        }      

        public void PlayCutscene(int id)
        {

        }

        public void GotoLobby()
        {

        }
    }

}
