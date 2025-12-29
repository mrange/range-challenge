using Spectre.Tui;
using System.Numerics;
using TermShader.Infrastructure;

using static Spectre.Tui.Color;

public sealed class Shader : ShaderBase
{
  protected override Color Run(int x, int y, int w, int h, TimeSpan elapsed)
  {
    Vector2 
      c = new (x,y)
    , r = new (w,h)
    , p = (c+c-r)/r.Y;
    ;

    float
      l = p.Length() - 1.0F
    ;

    return l<0?White:Black;
  }
}