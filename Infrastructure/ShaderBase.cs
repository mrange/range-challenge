using Spectre.Tui;

namespace TermShader.Infrastructure;

public abstract class ShaderBase
{
    public void Render(RenderContext context, double time)
    {
        int width = context.Viewport.Width;
        int height = context.Viewport.Height;

        Setup(width, height+height, time);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var c = context.GetCell(x,y);
                if (c is not null)
                {
                  c.SetSymbol('▀');
                  c.SetForeground(Run(x, y+y+0));
                  c.SetBackground(Run(x, y+y+1));
                }
            }
        }
    }

    protected abstract void Setup(int weight, int height, double time);
    protected abstract Color Run(int x, int y);
}