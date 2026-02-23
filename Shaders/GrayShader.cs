using Spectre.Tui;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using TermShader.Infrastructure;

using static System.MathF;
using static System.Numerics.Vector3;

public sealed class GrayShader : ShaderBase
{
  public override string Name { get; } = "Gray";

  static readonly Vector2 _path=MathF.Tau*new Vector2(1/11F,1/13F);

  // TODO: Remove these 2
  float _G;
  Vector3 _LP;

  Vector2 _R;
  Vector3 _S;
  Vector3 _X;
  Vector3 _Y;
  Vector3 _Z;

  protected override void Setup(int width, int height, double time)
  {
    var t=(float)(time%143);
    _R=new(width,height);
    Vector3
      o0
    , o1
    , o2
    ;
    OFF(t+3,out o0, out o1, out o2);
    _LP=o0;
    OFF(t,out o0, out o1, out o2);
    _S=o0;
    _Z=Normalize(o1);
    _X=Normalize(Cross(new Vector3(0,1,0)-o2,_Z));
    _Y=Cross(_X,_Z);

  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  static float L(Vector3 p)
  {
    p*=p;
    p*=p;
    return Sqrt(Sqrt(Sqrt(Dot(p,p))));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    R=Cos(new Vector3(o2.X)+new Vector3(0,11,33)).AsVector4();
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
    Vector3 
      E=new(1E-4F,0,0)
    ;
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
      C=new(x,_R.Y-y)
    , P=(2F*C-_R)/_R.Y
    ;

    Vector3
      S=_S
    , I=Normalize(-P.X*_X+P.Y*_Y+2*_Z)
    , p
    , D
    , n
    , o=Zero
    ;

    _G=1E3F;
    z=M(S,I);
    g=_G;
    p=FusedMultiplyAdd(new(z),I,S);
    D=Normalize(_LP-p);
    n=N(p);
    if(z<4&&n.Z>-.9F)
      o+=new Vector3(Pow(Max(0,Dot(Reflect(I,n),D)),40));
//    o+=new Vector3(1E-2F/Max(g,1E-3F));
    o=SquareRoot(o);
    o-=new Vector3(.07F);
    return ToColor(o);
  }
}