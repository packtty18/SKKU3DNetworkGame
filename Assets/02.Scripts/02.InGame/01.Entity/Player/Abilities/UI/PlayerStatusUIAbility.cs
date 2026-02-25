using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUIAbility : PlayerAbility
{
    [SerializeField] private Image _healthGuage;
    [SerializeField] private Image _staminaGuage;

    [SerializeField] private Color _normalStamniaColor = Color.orange;
    [SerializeField] private Color _exhaustedStaminaColor = Color.red;

    private void LateUpdate()
    {
        _healthGuage.fillAmount = _owner.GetAbility<PlayerHealthAbility>().Health.Ratio;
        _staminaGuage.fillAmount = _owner.GetAbility<PlayerStaminaAbility>().Stamina.Ratio;
        
        //추후 꾸민다면 게이지 자체가 아닌 프레임의 색이 변하도록 조정
        if(_owner.Exhausted)
        {
            _staminaGuage.color = _exhaustedStaminaColor;
        }
        else
        {
            _staminaGuage.color = _normalStamniaColor;
        }
    }
}