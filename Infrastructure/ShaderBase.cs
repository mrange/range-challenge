using Spectre.Tui;

namespace TermShader.Infrastructure;

public abstract class ShaderBase
{
    public void Render(RenderContext context, TimeSpan elapsed)
    {
        for (var x = 0; x < context.Viewport.Width; x++)
        {
            for (var y = 0; y < context.Viewport.Height; y++)
            {
                context.GetCell(x, y)
                    ?.SetBackground(Run(x, y, elapsed));
            }
        }
    }

    protected abstract Color Run(int x, int y, TimeSpan elapsed);
}