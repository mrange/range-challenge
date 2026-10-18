using Spectre.Tui;
using System.Numerics;
using System.Runtime.Intrinsics;
using TermShader.Infrastructure;

using static System.MathF;
using static System.Numerics.Vector3;

public sealed class CavernShader : ShaderBase
{
  Vector2 _R;
  Vector2 _R2;
  Vector3 _Off;

  public override string Name { get; } = "Submerged cavern of frozen flame";

  protected override void Setup(int width, int height, double time)
  {
    var t=(float)time;
    _R=new(width, height);
    _R2=.5F*_R;
    _Off=new(0,0,t/20);
  }

  protected override Color Run(int x, int y, Color previous)
  {
    Vector3
      p
    , P
    , R=Normalize(new(new Vector2(x,_R.Y-y)-_R2,_R.Y))
    , Off=_Off
    , Z
    ;
    Vector4
      o=Vector4.Zero
    , c
    , F=new(1,2,3,1)
    , B=new(1,2,3,0)
    ;

    float
      d
    , s
    , S
    , z=0
    ;
    for(int i=0;i<50;++i)
    {
      p=FusedMultiplyAdd(new(z),R, Off);
      s=p.Y+.1F;
      p.Y=Abs(s)-.1F;
      P=p;
      Z=new(P.Z);
      p+=Sin(FusedMultiplyAdd(new Vector3( 6F), Shuffle(p,1,2,0), Z))*(1F/ 6F);
      p+=Sin(FusedMultiplyAdd(new Vector3(12F), Shuffle(p,1,2,0), Z))*(1F/12F);
      p+=Sin(FusedMultiplyAdd(new Vector3(24F), Shuffle(p,1,2,0), Z))*(1F/24F);
      p+=Sin(FusedMultiplyAdd(new Vector3(48F), Shuffle(p,1,2,0), Z))*(1F/48F);
      S=(P-p).Length();
      s=s<0?.1F*s:0;
      d=Abs(0.63F-(p-Round(p)).Length()+s)+2e-3F-s;
      c=Vector4.One+Vector4.Sin(F+new Vector4(FusedMultiplyAdd(5, P.X, FusedMultiplyAdd(9, S, 0.5F*p.Z))));
      o+=Vector4.FusedMultiplyAdd(new(c.W*ReciprocalEstimate(d)*S*S),c,-Sign(s)*B);
      z+=d*.2F;
    }
    s=z*z;
    s*=s;
    o+=s*z*B;
    o*=2e-3F;
    return ToColor(TanhApprox(o.AsVector3()));
  }
}