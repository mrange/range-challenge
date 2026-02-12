using System.Diagnostics;
using TermShader.Infrastructure;
using System.Runtime.CompilerServices;

var shader = new SelectionPrompt<ShaderBase>()
    .Title(
    """
    A [blue]Spectre.Tui[/] demo by [yellow]Mårten Rånge[/]
    What app do you want to run?
    """)
    .UseConverter(c => c.Name)
    .AddChoices(
        new BoxShader(),
        new ApolloShader(),
        new LandscapeShader(),
        new GrottoShader())
    .Show(AnsiConsole.Console);

var isRunning = true;
Console.CancelKeyPress += (e, s) =>
{
    s.Cancel = true;
    isRunning = false;
};

using var terminal = Terminal.Create();
var renderer = new Renderer(terminal);
renderer.SetTargetFps(60);
var sw = Stopwatch.StartNew();

while (isRunning)
{
    renderer.Draw((ctx, elapsed) =>
    {
        shader.Render(ctx, sw.Elapsed.TotalSeconds);

        // Render FPS
        var fps = (int)(1 / elapsed.TotalSeconds);
        ctx.Render(Text.FromString($"{fps}", new Appearance
        {
            Foreground = GetFpsColor(fps),
        }));
    });

    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(true).Key;
        if (key is ConsoleKey.Escape or ConsoleKey.Q)
        {
            isRunning = false;
        }
    }
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static Color GetFpsColor(int fps)
{
    return fps switch
    {
        >= 59 => Color.Green,
        >= 24 => Color.Yellow,
        _ => Color.Red,
    };
}