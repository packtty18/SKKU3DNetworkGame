using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUIAbility : PlayerAbility
{
    [SerializeField] private Image _healthGuage;
    [SerializeField] private Image _staminaGuage;

    private void FixedUpdate()
    {
        _healthGuage.fillAmount = _owner.Health.Ratio;
        _staminaGuage.fillAmount = _owner.Stamina.Ratio;
    }
}