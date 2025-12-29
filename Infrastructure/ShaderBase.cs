using Spectre.Tui;

namespace TermShader.Infrastructure;

public abstract class ShaderBase
{
    public void Render(RenderContext context, TimeSpan elapsed)
    {
        int width = context.Viewport.Width;
        int height = context.Viewport.Height;
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var c = context.GetCell(x,y);
                if (c is not null)
                {
                  c.SetSymbol('▀');
                  c.SetForeground(Run(x, y+y+0, width, height+height, elapsed));
                  c.SetBackground(Run(x, y+y+1, width, height+height, elapsed));
                }
            }
        }
    }

    protected abstract Color Run(int x, int y, int weight, int height, TimeSpan elapsed);
}