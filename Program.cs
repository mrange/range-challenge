using Spectre.Tui;

var terminal = Terminal.Create();
var renderer = new Renderer(terminal);
var shader = new Shader();

while (true)
{
    renderer.Draw((ctx, elapsed) =>
    {
        shader.Render(ctx, elapsed);
    });
    
    // Check for exit
    if (Console.KeyAvailable)
    {
        if (Console.ReadKey(true).Key == ConsoleKey.Q)
        {
            break;
        }
    }
}