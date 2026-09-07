using GameEventSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace S7
{
    public partial class Global : Singleton<Global>
    {
        private Dictionary<long, CharacterData> _characters = new Dictionary<long, CharacterData>();

        private void InitializeCharacter()
        {

        }

        public void AddCharacter(List<CharacterData> characters)
        {
            _characters.Clear();
            foreach (var character in characters)
            {
                AddCharacter(character);
            }
        }

        public void AddCharacter(CharacterData data)
        {
            if (_characters.ContainsKey(data.unitKey) == false)
            {
                return;
            }

            _characters.Add(data.unitKey, data);

            //°»½Å
        }

        public void UpdateCharacterData(CharacterData data)
        {
            if (_characters.ContainsKey(data.unitKey))
            {
                _characters[data.unitKey] = data;
            }
            else
            {
                AddCharacter(data);
            }

            EventManager.BroadCasting(new EventTarget(GameEventSystem.EventType.UpdataCharacter), data);
        }

        public bool GetCharacter(long unitKey, out CharacterData data)
        {
            if (_characters.ContainsKey(unitKey))
            {
                data = _characters[unitKey];
                return true;
            }

            data = null;
            return false;
        }



    }
}
