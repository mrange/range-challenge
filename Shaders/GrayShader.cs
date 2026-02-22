using Spectre.Tui;
using System.Numerics;
using System.Runtime.Intrinsics;
using TermShader.Infrastructure;

using static System.MathF;
using static System.Numerics.Vector3;

public sealed class GrayShader : ShaderBase
{
  public override string Name { get; } = "Gray";

  static readonly Vector2 _path=MathF.Tau*new Vector2(1/11F,1/13F);

  float _Z;
  float _G;
  Vector2 _R;
  Vector3 _LP;

  protected override void Setup(int width, int height, double time)
  {
    _G=default;
    _LP=default;
    _Z=default;
  }

  static float L(Vector3 p)
  {
    p*=p;
    p*=p;
    return Sqrt(Sqrt(Sqrt(Dot(p,p))));
  }

  static void OFF(float z, out Vector3 o0)
  {
    Vector2 
      p=_path
    , s=.5F*Vector2.Sin(p*z)
    ;
    o0=new(s,z);
  }

  static void OFF(float z, out Vector3 o0, out Vector3 o1, out Vector3 o2)
  {
    Vector2 
      p=_path
    , c=.5F*Vector2.Cos(p*z)
    , s=.5F*Vector2.Sin(p*z)
    ;
    o0=new(s,z);
    o1=new(p*c,1);
    o2=new(p*p*c,0);
  }

  float D(Vector3 p)
  {
    float
      d=0
    , g=(p-_LP).Length()-.03F
    ;
    Vector3
      o0
    , o1
    , o2
    ;

    Vector4
      P
    , R
    , M=Vector128.Create(0,0,~0,~0).AsSingle().AsVector4()
    ;

    OFF(p.Z,out o0,out o1,out o2);
    o1=Normalize(o1);
    p-=o0.AsVector2().AsVector3();
    p-=Dot(p.AsVector2().AsVector3(), o1)*new Vector3(.5F,.5F,-.5F)*o1;
    R=Cos(new Vector3(p.Z+p.Z)+new Vector3(0,11,33)).AsVector4();
    P=p.AsVector4();
    P=Vector4.FusedMultiplyAdd(Vector4.Shuffle(R,2,1,3,3),Vector4.Shuffle(P,1,0,2,3),Vector4.FusedMultiplyAdd(Vector4.Shuffle(R,0,0,3,3),P,Vector4.BitwiseAnd(P,M)));
    p=P.AsVector3();

    p-=Round(p);
    d=Max(d,.37F-L(p));
    p-=Round(2F*p)*.5F;
    d=Max(d,.5F*.37F-L(p));
    _G=Min(_G,g);
    d=Min(d,g);
    return d;
  }

  Vector3 N(Vector3 p)
  {
    Vector3 E=new(1E-4F,0,0);
    return Normalize(new(
      D(p+E)-D(p-E)
    , D(p+Shuffle(E,1,0,2))-D(p-Shuffle(E,1,0,2))
    , D(p+Shuffle(E,2,1,0))-D(p-Shuffle(E,2,1,0))
    ));
  }

  float M(Vector3 S, Vector3 I)
  {
    float
      z=0
    , d
    ;

    for(int i=0;i<66;++i)
    {
      d=D(FusedMultiplyAdd(new(z),I,S));
      if(d<1E-3F||z>4) break;
      z+=d;
    }

    return z;
  }

  protected override Color Run(int x, int y, Color previous)
  {
    float
      z
    , g
    ;

    Vector2
      C=new(x,y)
    , P=(2F*C-_R)/_R.Y
    ;

    Vector3
      o0
    , o1
    , o2
    ;

    OFF(_Z,o0,o1,o2);

    return ToColor(Vector3.Zero);
  }
}