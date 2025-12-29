using Spectre.Tui;
using TermShader.Infrastructure;

public sealed class Shader : ShaderBase
{
    protected override Color Run(int x, int y, TimeSpan elapsed)
    {
        return Random.Shared.Next(0, 255);
    }
}