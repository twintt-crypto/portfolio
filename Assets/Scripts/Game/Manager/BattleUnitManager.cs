using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace S7
{
    public class BattleUnitManager
    {
        private readonly List<UnitController> _allies = new();
        private readonly List<UnitController> _enemies = new();
        private readonly IReadOnlyList<UnitData> _allyUnits;
        private readonly IReadOnlyList<UnitData> _enemyUnits;

        public IReadOnlyList<UnitController> Allies => _allies;
        public IReadOnlyList<UnitController> Enemies => _enemies;
        public IEnumerable<UnitController> AllUnits => _allies.Concat(_enemies);

        public BattleUnitManager(IReadOnlyList<UnitData> allyUnits, IReadOnlyList<UnitData> enemyUnits)
        {
            _allyUnits = allyUnits;
            _enemyUnits = enemyUnits;
        }

        public async UniTask BuildUnits(BattleStage battleStage)
        {
            await BuildAllies(battleStage);
            await BuildEnemies(battleStage);
        }

        async UniTask BuildAllies(BattleStage battleStage)
        {
            _allies.Clear();

            foreach (UnitData item in _allyUnits)
            {
                var view = await SpawnUnitView(item, battleStage.GetSlot(BattleSide.Ally, item.fomationIndex));
                List<UnitSkill> skills = item is CharacterData characterData
                    ? CreateCharacterSkills(characterData)
                    : new List<UnitSkill>();
                var unitController = new UnitController(item, skills, view);
                _allies.Add(unitController);
            }
        }

        async UniTask BuildEnemies(BattleStage battleStage)
        {
            _enemies.Clear();

            foreach (UnitData item in _enemyUnits)
            {
                var view = await SpawnUnitView(item, battleStage.GetSlot(BattleSide.Enemy, item.fomationIndex));
                List<UnitSkill> skills = item is MonstarData monstarData
                    ? CreateMonsterSkills(monstarData)
                    : new List<UnitSkill>();
                var controller = new UnitController(item, skills, view);
                _enemies.Add(controller);
            }
        }

        private async Task<UnitView> SpawnUnitView(UnitData data, BattleSlot slot)
        {
            T_UnitData unit = T_UnitData.Get(data.unitId);
            if (unit == null)
            {
                return null;
            }

            GameObject viewObject = await ResourceManager.NewAsync("UnitView", slot.transform);
            if (viewObject == null)
            {
                return null;
            }

            UnitView view = viewObject.GetComponent<UnitView>();
            if (view == null)
            {
                return null;
            }

            await view.LoadModelAsync(unit.ModelPrefab);
            view.Initialize(data, slot.targetPoint);
            return view;
        }

        private List<UnitSkill> CreateCharacterSkills(CharacterData data)
        {
            var skills = new List<UnitSkill>();

            T_CharacterData characterData = T_CharacterData.Get(data.unitId);
            if (characterData != null)
            {
                //�Ϲݰ���
                {
                    var skillId = characterData.AttackSkillId;
                    var skillData = T_SkillData.Get(skillId);
                    if (skillData != null)
                    {
                        UnitSkill skill = new();

                        skill.skillData = skillData;
                        skill.remainingCooldownTurns = 0;

                        skills.Add(skill);
                    }
                }

                //��ų����
                {
                    var skillId = characterData.SkillAttackSkillId;
                    var skillData = T_SkillData.Get(skillId);
                    if (skillData != null)
                    {
                        UnitSkill skill = new();

                        skill.skillData = skillData;
                        skill.remainingCooldownTurns = 0;

                        skills.Add(skill);
                    }                    
                }

                //����� 
                {
                    var skillId = characterData.SpecialSkill;
                    var skillData = T_SkillData.Get(skillId);
                    if (skillData != null)
                    {
                        UnitSkill skill = new();

                        skill.skillData = skillData;
                        skill.remainingCooldownTurns = 0;

                        skills.Add(skill);
                    }                    
                }

                //�ñر� 
                {
                    var skillId = characterData.UltimateSkillId;
                    var skillData = T_SkillData.Get(skillId);
                    if (skillData != null)
                    {
                        UnitSkill skill = new();

                        skill.skillData = skillData;
                        skill.remainingCooldownTurns = 0;

                        skills.Add(skill);
                    }                    
                }

                //�нú�
                {
                    var skillId = characterData.PassiveSkillId;
                    var skillData = T_SkillData.Get(skillId);
                    if (skillData != null)
                    {
                        UnitSkill skill = new();

                        skill.skillData = skillData;
                        skill.remainingCooldownTurns = 0;

                        skills.Add(skill);
                    }                    
                }
            }

            return skills;
        }

        private List<UnitSkill> CreateMonsterSkills(MonstarData data)
        {
            T_MonsterPresetData monsterPresetData = T_MonsterPresetData.Get(data.unitId);
            if (monsterPresetData == null)
            {
                return null;
            }

            var skills = new List<UnitSkill>();

            foreach (var id in monsterPresetData.SkilID)
            {
                var baseSkill = T_SkillData.Get(id);
                if (baseSkill == null)
                {
                    continue;
                }

                UnitSkill unitSkill = new UnitSkill
                {
                    skillData = baseSkill,
                    remainingCooldownTurns = 0
                };

                skills.Add(unitSkill);
            }

            return skills;
        }
    }
}
