using System.ComponentModel.DataAnnotations;
using NetEscapades.EnumGenerators;

namespace KPO_HW2.Models;

[EnumExtensions]
public enum CurrencyCode
{
    [Display(Name = "USD")]
    Usd,

    [Display(Name = "RUB")]
    Rub
}