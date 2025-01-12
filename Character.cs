using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace XIYOU
{
    class Character
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public int EB { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public PictureBox PictureBox { get; set; }

        // 主动技能
        public Action<Character> ActiveSkill1 { get; set; }
        public Action<Character> ActiveSkill2 { get; set; }
        public Action<Character> ActiveSkill3 { get; set; }

        // 技能名称
        public string Skill1Name { get; set; }
        public string Skill2Name { get; set; }
        public string Skill3Name { get; set; }

        // 被动技能
        public Action<Character> PassiveSkill1 { get; set; }
        public Action<Character> PassiveSkill2 { get; set; }

        // 被动技能名称
        public string PassiveSkill1Name { get; set; }
        public string PassiveSkill2Name { get; set; }

        // 日志更新委托
        public Action<string, Color> UpdateBattleLog { get; set; } // 修改为使用 Color
        public Color LogColor { get; set; } // 新增日志颜色属性

        public Character(string name, int hp, int eb, int attack, int defense, PictureBox pictureBox)
        {
            Name = name;
            HP = hp;
            EB = eb;
            Attack = attack;
            Defense = defense;
            PictureBox = pictureBox;

            // 初始化技能名称
            Skill1Name = "技能1";
            Skill2Name = "技能2";
            Skill3Name = "技能3";

            // 初始化被动技能名称
            PassiveSkill1Name = "默认被动技能1";
            PassiveSkill2Name = "默认被动技能2";
        }

        // 移动角色到指定位置
        public void Move(int x, int y)
        {
            if (PictureBox != null && !PictureBox.IsDisposed)
            {
                if (PictureBox.InvokeRequired)
                {
                    PictureBox.Invoke(new Action(() => Move(x, y)));
                }
                else
                {
                    PictureBox.Location = new Point(x * 50, y * 50);
                }
            }
        }

        // 攻击目标角色
        public void AttackTarget(Character target)
        {
            int damage = Math.Max(1, Attack - target.Defense); // 确保伤害至少为 1
            target.HP -= damage;

            // 触发被动技能
            if (PassiveSkill1 != null)
            {
                PassiveSkill1(target);
               
            }
            if (PassiveSkill2 != null)
            {
                PassiveSkill2(target);
              
            }

            // 更新日志
            UpdateBattleLog?.Invoke($"{Name} 攻击了 {target.Name}，造成 {damage} 点伤害", target.LogColor);

            // 检查目标是否阵亡，并执行 Defense 到 HP 的转换
            target.IsDead();
        }

        // 判断角色是否阵亡，并将 Defense 转化为 HP
        public bool IsDead()
        {
            if (HP <= 0 && Defense > 0)
            {
                HP += Defense; // 将 Defense 转化为 HP
                Defense = 0;   // Defense 清零
                UpdateBattleLog?.Invoke($"{Name} 的 Defense 转化为 HP，当前 HP: {HP}",LogColor);
            }
            return HP <= 0;
        }

        // 使用主动技能
        public void UseActiveSkill(int skillIndex, Character target)
        {
            string skillName = "";
            Action<Character> skillAction = null;

            switch (skillIndex)
            {
                case 1:
                    skillName = Skill1Name;
                    skillAction = ActiveSkill1;
                    break;
                case 2:
                    skillName = Skill2Name;
                    skillAction = ActiveSkill2;
                    break;
                case 3:
                    skillName = Skill3Name;
                    skillAction = ActiveSkill3;
                    break;
                default:
                    throw new ArgumentException("无效的技能");
            }

            if (skillAction != null)
            {
                skillAction(target);
                UpdateBattleLog?.Invoke($"{Name} 使用了 {skillName} 攻击 {target.Name}", LogColor);
            }
        }

        // 使用随机技能
        public void UseRandomSkill(Character target)
        {
            Random random = new Random();
            int skillIndex = random.Next(1, 4);
            UseActiveSkill(skillIndex, target);
        }
    }
}

