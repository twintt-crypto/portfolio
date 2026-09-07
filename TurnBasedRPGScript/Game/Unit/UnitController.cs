using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace S7
{
    public class UnitController
    {
        // ================================
        // Core
        // ================================
        public UnitData data { get; }
        public UnitView view { get; }

        public Dictionary<int, UnitSkill> skills = new();

        public BuffManager BuffManager { get; private set; }

        // ================================
        // Constructor
        // ================================
        public UnitController(UnitData data, List<UnitSkill> sklls, UnitView view)
        {
            this.data = data;
            this.view = view;

            skills.Clear();
            foreach (var skill in sklls)
            {
                skills.Add(skill.skillData.TID, skill);
            }

            this.view.Bind(this);
            this.BuffManager = new BuffManager(this);
            
            data.BindBuffManager(BuffManager);            
        }

        public bool CanParticipateBattle()
        {
            return true/* 전투 참여 가능 여부 */;
        }

        public void ApplyDamage(int damage)
        {
            data._stat.hp -= damage;

            //await BattleUIManager.Instance.ShowDamage(view.HitPoint.position, hitResult.damage);

            /*if (hitResult.isCritical == true)
            {

            }

            if (data._stat.hp < 0)
            {
                data._stat.hp = 0;
                view.PlayDeath();
            }*/
        }


        private void Die()
        {
            /*view.PlayDeath();
            OnDead?.Invoke(this);*/
        }

        // ================================
        // Utility
        // ================================
        public void Heal(int value)
        {
            /*if (!IsAlive)
                return;

            data.HP += value;
            if (data.HP > data.MaxHP)
                data.HP = data.MaxHP;*/
        }


        public void Parrying()
        {

        }

        public void Jump()
        {

        }

        public bool IsDead()
        {
            return data.IsDead;
        }

        public bool CanAct()
        {
            return true;
        }

        public void OnTurnStart()
        {

        }

        public void OnTurnEnd()
        {

        }

        public void CheckDead()
        {
            if(data.IsDead == true)
            {
                view.PlayDeath();
            }
        }

        public UnitSkill GetAttackSkill()
        {
            T_CharacterData characterData = T_CharacterData.Get(data.unitId);
            if (characterData == null)
                return null;

            if(skills.TryGetValue(  characterData.AttackSkillId, out UnitSkill skill ) == false)
            {
                return null;
            }

            return skill;
        }

        public UnitSkill GetUltimateSkill()
        {
            T_CharacterData characterData = T_CharacterData.Get(data.unitId);
            if (characterData == null)
                return null;

            if (skills.TryGetValue(characterData.UltimateSkillId, out UnitSkill skill) == false)
            {
                return null;
            }

            return skill;
        }
    }
}

