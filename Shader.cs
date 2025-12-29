using Spectre.Tui;
using System.Numerics;
using TermShader.Infrastructure;

using static System.Math;

public sealed class Shader : ShaderBase
{
  readonly static Vector3 _0  = new (0);
  readonly static Vector3 _1  = new (1);
  readonly static Vector3 _255= new (255);

  static Color ToColor(Vector3 c) 
  {
    var C=Vector3.Clamp(c,_0,_1)*_255;
    return new((byte)C.X,(byte)C.Y,(byte)C.Z);
  }

  float   _time;
  Vector2 _res;

  protected override void Setup(int weight, int height, double time)
  {
    _time=(float)time;
    _res =new(weight, height);
  }

  protected override Color Run(int x, int y)
  {
    Vector2 
      c = new (x,y)
    , p = (c+c-_res)/_res.Y;
    ;

    Vector3
      col = _0
    ;

    float
      b
    , l
    ;
    b = _time-(float)Floor(_time)-.5F;
    b *=b;
    b = .25F-b;
    p.Y+=b;
    l = p.Length() - .5F;

    if (l<0) 
    {
      col = _1;
    }

    return ToColor(col);
  }
}