using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_test
{
    // Ability 클래스
    public class Ability
    {
        AbilityType type;
        string name;

        public Ability(AbilityType abilityType)
        {
            type = abilityType;

            if (type == AbilityType.darkUnderTheLamp)
                name = "등잔 밑이 어둡다";         

            else if (type == AbilityType.galaxyTravel)
                name = "은하 탐방";         // work.cs 구현

            else if (type == AbilityType.planetTravel)
                name = "행성 탐방";         // work.cs 구현

            else if (type == AbilityType.stun)      
                name = "스턴";            // work.cs 구현 - 이건 게임 구현 후 턴 개념 추가되면 재구현

            else if (type == AbilityType.handcuff)
                name = "수갑";        // work.cs 반 정도 구현 - TODO

            else if (type == AbilityType.teamIdentify)
                name = "팀 식별";          // work.cs 구현

            else if (type == AbilityType.getFuel)
                name = "겟 퓨얼";          // work.cs 구현

            else if (type == AbilityType.fuelChanger)
                name = "연료 교환권";        // work.cs 구현

            else if (type == AbilityType.fuelCompressor)
                name = "연료 압축기";        // work.cs 일단 구현..?


            else if (type == AbilityType.stunRemover)
                name = "스턴 제거기";            // work.cs 구현

            else if (type == AbilityType.storageGrowth)
                name = "저장량 증가";        // work.cs 구현
        }

        public AbilityType GetAbilityType()
        {
            return type;
        }

        public string GetAbilityname()
        {
            return name;
        }
    }

    public enum AbilityType
    {
        // 경찰
        darkUnderTheLamp,   // 등잔 밑이 어둡다
        galaxyTravel,         // 은하 탐방
        planetTravel,         // 행성 탐방
        stun,                  // 스턴
        handcuff,              // 수갑
        teamIdentify,         // 팀 식별

        // 도둑
        getFuel,              // 겟 퓨얼
        fuelChanger,          // 연료 교환권
        fuelCompressor,       // 연료 압축기
        stunRemover,          // 스턴 제거기
                               
        // 공통
        storageGrowth           // 저장량 증가
    }
}