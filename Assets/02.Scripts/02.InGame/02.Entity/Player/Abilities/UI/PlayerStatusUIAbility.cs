using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUIAbility : PlayerAbility
{
    [SerializeField] private Image _healthGuage;
    [SerializeField] private Image _staminaGuage;

    [SerializeField] private Color _normalStamniaColor = Color.darkTurquoise;

    private void LateUpdate()
    {
        _healthGuage.fillAmount = _owner.GetAbility<PlayerHealthAbility>().Health.Ratio;
        _staminaGuage.fillAmount = _owner.GetAbility<PlayerStaminaAbility>().Stamina.Ratio;
    }
}