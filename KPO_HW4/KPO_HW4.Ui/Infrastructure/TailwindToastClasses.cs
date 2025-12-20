namespace KPO_HW4.Ui.Infrastructure;

public static class TailwindToastClasses
{
    private const string Base =
        "pointer-events-auto w-[360px] max-w-full rounded-xl border shadow-lg " +
        "px-4 py-3 text-sm text-gray-900 bg-white " +
        "flex gap-3 items-start " +
        "backdrop-blur " +
        "transition";

    public static readonly string Info =
        $"{Base} border-blue-200 " +
        "before:content-[''] before:w-1 before:self-stretch before:rounded-l-xl before:bg-blue-500";

    public static readonly string Success =
        $"{Base} border-emerald-200 " +
        "before:content-[''] before:w-1 before:self-stretch before:rounded-l-xl before:bg-emerald-500";

    public static readonly string Warning =
        $"{Base} border-amber-200 " +
        "before:content-[''] before:w-1 before:self-stretch before:rounded-l-xl before:bg-amber-500";

    public static readonly string Error =
        $"{Base} border-red-200 " +
        "before:content-[''] before:w-1 before:self-stretch before:rounded-l-xl before:bg-red-500";
}