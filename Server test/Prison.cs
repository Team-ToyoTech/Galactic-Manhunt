using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_test
{
    // 잡힌 도둑들 가두어놓는 감옥 클래스
    public class Prison
    {
        public int maxCount;
        public int robberCount = 0;
        public Galaxy galaxy;

        public Prison(Galaxy galaxy)
        {
            this.galaxy = galaxy;
            maxCount = Server.robbers.Count();
        }

        public void AddRobber()
        {
            robberCount++;
        }

        public bool IsFinish()
        {
            if (robberCount == maxCount)
            {
                return true;
            }
            return false;
        }
        public void MinusRobber()
        {
            robberCount--;
        }
    }
}
