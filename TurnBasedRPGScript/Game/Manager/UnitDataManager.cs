using System.Collections.Generic;

namespace S7
{
    public class UnitDataManager : Singleton<UnitDataManager>
    {
        private readonly List<UnitData> _partyUnits = new();

        public IReadOnlyList<UnitData> PartyUnits => _partyUnits;
        public bool HasParty => _partyUnits.Count > 0;
        
        public void InitializeParty(List<UnitData> partyUnits)
        {
            _partyUnits.Clear();

            for (int i = 0; i < partyUnits.Count; i++)
            {
                partyUnits[i].fomationIndex = i;
                _partyUnits.Add(partyUnits[i]);
            }
        }

        public void ClearParty()
        {
            _partyUnits.Clear();
        }

        public void InitializeTestParty()
        {
            List<UnitData> testParty = new();
            for (int i = 0; i < 4; i++)
            {
                CharacterData characterData = new CharacterData();
                characterData.Initiaize(i + 1, 1, 1, 0);
                testParty.Add(characterData);
            }
            InitializeParty(testParty);
        }

        public static List<UnitData> CreateTestEnemies()
        {
            List<UnitData> enemies = new();
            for (int i = 0; i < 4; i++)
            {
                MonstarData monstarData = new MonstarData();
                monstarData.Initiaize(i + 5, 2);
                monstarData.fomationIndex = i;
                enemies.Add(monstarData);
            }
            return enemies;
        }
    }
}
