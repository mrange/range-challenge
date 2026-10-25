using MiniAudioEx.Core.StandardAPI;
using MiniAudioEx.Native;
using MiniAudioEx.Utilities;
using Spectre.Tui;
using System.Numerics;
using TermShader.Infrastructure;

using static System.MathF;
using static System.Numerics.Vector3;

public sealed class ApolloShader : ShaderBase
{
  readonly static Vector3 _Base = new(2,1,0);

  float   _inv;
  float   _fad;
  float   _sin;
  Vector2 _res;
  Vector3 _rot;

  readonly AudioApp _app;
  readonly float[]  _data;
  readonly int      _channels;
  AudioSource?      _source;
  volatile int      _pos;

  public ApolloShader()
  {
    var decoder = new AudioDecoder();
    decoder.DecodeFromFile(@"D:\code\github\range-challenge\michael-burns.mp3");
    var channels    = decoder.Channels;
    var data        = decoder.Data;
    var sampleRate  = decoder.SampleRate;
    _data           = data;
    _channels       = channels;
    _app            = new AudioApp((uint)sampleRate, (uint)channels);
    _app.Loaded     += OnAppLoaded;

    var thread = 
      new Thread(_app.Run)
      {
        IsBackground = true
      };
    thread.Start();
  }

  void OnAppLoaded()
  {
    if (_source is null)
    {
      _source = new();
      _source.Read += OnSourceRead;
      _source.Play();
    }
  }

  unsafe void OnSourceRead(NativeArray<float> framesOut, ulong frameCount, int channels)
  {
    if (_source is not null)
    {
      var flen = framesOut.Length;
      var len = Math.Min(flen, _data.Length - _pos);
      var rem = flen - len;
      if (len > 0)
      {
        fixed (float * pdata = _data)
        {
          var na = new NativeArray<float>(pdata+_pos, len);
          na.CopyTo(framesOut);
        }
      }
      _pos += len;

      if (rem > 0)
      {
        for (int i = len; i < flen; ++i)
        {
          framesOut[i]=0;
        }
        _pos = 0;
      }
    }
  }

  public override string Name { get; } = "Fractals";

  protected override void Setup(int width, int height, double time)
  {
    var t=(float)time;
    _res=new(width, height);
    _inv=1/_res.Y;
    _rot=Normalize(Sin(new Vector3(.2F*t+123)+new Vector3(0,1,2)));
    _sin=Sin(.123F*t);
    _fad=.5F;
  }

  protected override Color Run(int x, int y, Color previous)
  {
    Vector2
      c=new (x,y)
    , p=(c+c-_res)*_inv
    ;

    Vector3
      P=new(p.X,p.Y,.5F*_sin)
    ;

    float
      s=1
    , k
    ;

    P=Dot(P,_rot)*_rot+Cross(P,_rot);

    for(int i=0; i<3;++i)
    {
      P-=2F*Round(.5F*P);
      k=1.41F/Dot(P,P);
      P*=k;
      s*=k;
    }

    P=Abs(P)/s;
    k=Min(P.Z, new Vector2(P.X,P.Y).Length());

    return ToColor(
        k<5E-3F
      ? One
      : _fad/(1+k*k*5)*(One+Sin(_Base+new Vector3(2+Log2(k))))
      );
  }
}