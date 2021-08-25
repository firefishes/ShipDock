namespace Elimlnate
{
    public class ComboRule
    {
        public string ruleName;
        public int linedCombo;

        public virtual void CreateSkillGrid(ref ElimlnateGrid rootGrid)
        {
        }

        public virtual bool ShouldCoverRule(ref ComboRule rule)
        {
            return rule != default && rule != this && rule.RuleLevel < RuleLevel;
        }

        public virtual int RuleLevel
        {
            get
            {
                return linedCombo;
            }
        }
    }
}
