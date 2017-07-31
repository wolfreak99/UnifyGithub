// Original url: http://wiki.unity3d.com/index.php/CustomRandom
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/CustomRandom.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
Overview Author: oPless 
A C# Portable random number generator, cut and pasted from an old library which in itself has been through umpteen revisions, converted from java and now uses ISAAC and also has been converted from java. feel free to clean up the code and add to it :) 
C# using System;
//ISAAC PRNG nabbed from http://www.burtleburtle.net/bob/rand/isaacafa.html
 
        public class CustomRandom //: PhysicalConstants
        {
                public virtual long Seed
                {
                        get
                        {
                                return origSeed;
                        }
 
                }
                protected internal bool normdone = false;
                protected internal double normstore;
                private long origSeed = 0;
 
      public const int SIZEL = 8;              /* log of size of rsl[] and mem[] */
      public const int SIZE = 1<<SIZEL;               /* size of rsl[] and mem[] */
      public const int MASK = (SIZE-1)<<2;            /* for pseudorandom lookup */
      public int count;                           /* count through the results in rsl[] */
      public int[] rsl;                                /* the results given to the user */
      private int[] mem;                                   /* the internal state */
      private int a;                                              /* accumulator */
      private int b;                                          /* the last result */
      private int c;              /* counter, guarantees cycle is at least 2^^40 */
 
 
                /// <summary> Default constructor.  Calls the constructor for Random with no seed.
                /// </summary>
                public CustomRandom():base()
                {
                mem = new int[SIZE];
            rsl = new int[SIZE];
                Init(false);
                }
 
                public CustomRandom(int[] seed) 
                {
                mem = new int[SIZE];
                rsl = new int[SIZE];
                for (int i=0; i<seed.Length; ++i) {
                  rsl[i] = seed[i];
                }
                Init(true);
        }
 
 
                /// <summary> Default constructor.  Calls the constructor for Random with the
                /// specified seed.
                /// </summary>
                public CustomRandom(long seed)
                {
                mem = new int[SIZE];
                rsl = new int[SIZE];
 
                        for(int i=0;i<8;i++)
                                rsl[i] = (byte) ( (seed >> (i*8) ) & 0xFFL );
 
                        origSeed = seed;
                        Init(true);
                }
 
      /* Generate 256 results.  This is a fast (not small) implementation. */
      public /*final*/ void Isaac() {
        int i, j, x, y;
 
        b += ++c;
        for (i=0, j=SIZE/2; i<SIZE/2;) {
          x = mem[i];
          a ^= a<<13;
          a += mem[j++];
          mem[i] = y = mem[(x&MASK)>>2] + a + b;
          rsl[i++] = b = mem[((y>>SIZEL)&MASK)>>2] + x;
 
          x = mem[i];
          a ^= (int)((uint)a>>6);
          a += mem[j++];
          mem[i] = y = mem[(x&MASK)>>2] + a + b;
          rsl[i++] = b = mem[((y>>SIZEL)&MASK)>>2] + x;
 
          x = mem[i];
          a ^= a<<2;
          a += mem[j++];
          mem[i] = y = mem[(x&MASK)>>2] + a + b;
          rsl[i++] = b = mem[((y>>SIZEL)&MASK)>>2] + x;
 
          x = mem[i];
          a ^= (int)((uint)a>>16);
          a += mem[j++];
          mem[i] = y = mem[(x&MASK)>>2] + a + b;
          rsl[i++] = b = mem[((y>>SIZEL)&MASK)>>2] + x;
        }
 
 
        for (j=0; j<SIZE/2;) {
          x = mem[i];
          a ^= a<<13;
          a += mem[j++];
          mem[i] = y = mem[(x&MASK)>>2] + a + b;
          rsl[i++] = b = mem[((y>>SIZEL)&MASK)>>2] + x;
 
          x = mem[i];
          a ^= (int)((uint)a>>6);
          a += mem[j++];
          mem[i] = y = mem[(x&MASK)>>2] + a + b;
          rsl[i++] = b = mem[((y>>SIZEL)&MASK)>>2] + x;
 
          x = mem[i];
          a ^= a<<2;
          a += mem[j++];
          mem[i] = y = mem[(x&MASK)>>2] + a + b;
          rsl[i++] = b = mem[((y>>SIZEL)&MASK)>>2] + x;
 
          x = mem[i];
          a ^= (int)((uint)a >> 16);
          a += mem[j++];
          mem[i] = y = mem[(x&MASK)>>2] + a + b;
          rsl[i++] = b = mem[((y>>SIZEL)&MASK)>>2] + x;
        }
      }
 
      /* initialize, or reinitialize, this instance of rand */
      public /*final*/ void Init(bool flag) {
        int i;
        int a,b,c,d,e,f,g,h;
        a=b=c=d=e=f=g=h=unchecked((int)0x9e3779b9);                        /* the golden ratio */
 
        for (i=0; i<4; ++i) {
          a^=b<<11;  d+=a; b+=c;
          b^=(int)((uint)c>>2);  e+=b; c+=d;
          c^=d<<8;   f+=c; d+=e;
          d^=(int)((uint)e>>16); g+=d; e+=f;
          e^=f<<10;  h+=e; f+=g;
          f^=(int)((uint)g>>4);  a+=f; g+=h;
          g^=h<<8;   b+=g; h+=a;
          h^=(int)((uint)a>>9);  c+=h; a+=b;
 
          normdone = false;//hmm?
        }
 
        for (i=0; i<SIZE; i+=8) {              /* fill in mem[] with messy stuff */
          if (flag) {
            a+=rsl[i  ]; b+=rsl[i+1]; c+=rsl[i+2]; d+=rsl[i+3];
            e+=rsl[i+4]; f+=rsl[i+5]; g+=rsl[i+6]; h+=rsl[i+7];
          }
          a^=b<<11;  d+=a; b+=c;
          b^=(int)((uint)c>>2);  e+=b; c+=d;
          c^=d<<8;   f+=c; d+=e;
          d^=(int)((uint)e>>16); g+=d; e+=f;
          e^=f<<10;  h+=e; f+=g;
          f^=(int)((uint)g>>4);  a+=f; g+=h;
          g^=h<<8;   b+=g; h+=a;
          h^=(int)((uint)a>>9);  c+=h; a+=b;
          mem[i  ]=a; mem[i+1]=b; mem[i+2]=c; mem[i+3]=d;
          mem[i+4]=e; mem[i+5]=f; mem[i+6]=g; mem[i+7]=h;
        }
 
        if (flag) {           /* second pass makes all of seed affect all of mem */
          for (i=0; i<SIZE; i+=8) {
            a+=mem[i  ]; b+=mem[i+1]; c+=mem[i+2]; d+=mem[i+3];
            e+=mem[i+4]; f+=mem[i+5]; g+=mem[i+6]; h+=mem[i+7];
            a^=b<<11;  d+=a; b+=c;
            b^=(int)((uint)c>>2);  e+=b; c+=d;
            c^=d<<8;   f+=c; d+=e;
            d^=(int)((uint)e>>16); g+=d; e+=f;
            e^=f<<10;  h+=e; f+=g;
            f^=(int)((uint)g>>4);  a+=f; g+=h;
            g^=h<<8;   b+=g; h+=a;
        h ^= (int)((uint)a >> 9); c += h; a += b;
            mem[i  ]=a; mem[i+1]=b; mem[i+2]=c; mem[i+3]=d;
            mem[i+4]=e; mem[i+5]=f; mem[i+6]=g; mem[i+7]=h;
          }
        }
 
        Isaac();
        count = SIZE;
      }
 
      /* Call rand.val() to get a random value */
      public /*final*/ long  val() 
      {
        if (0 == count--) {
          Isaac();
          count = SIZE-1;
        }
        return (rsl[count] & 0xFFFFFFFFL);
      }
 
 
                public double NextDouble() 
                {
                        double d=0;
                        long   longish = val();
                        long   maxint  = (-1 & 0xFFFFFFFFL);
                        d = ((double) longish) / ((double) maxint);
                        //              if (prngstate == 2) { prngdata = new byte[16];prngdata = prng.ComputeHash(prngdata); prngstate=0; }
                //      Console.Out.WriteLine("random number: {0} / {1} : {2}",d,longish, maxint);
                        return d;
                }
 
               /// <summary> Produces a Gaussian random variate with mean=0, standard deviation=1.
                /// </summary>
                public virtual double NormalDeviate()
                {
                        double v1, v2, r, fac;
 
                        if (normdone)
                        {
                                normdone = false;
                                return normstore;
                        }
                        else
                        {
                                v1 = 2.0 * NextDouble() - 1.0;
                                v2 = 2.0 * NextDouble() - 1.0;
                                r = v1 * v1 + v2 * v2;
                                if (r >= 1.0)
                                {
                                        return NormalDeviate();
                                }
                                else
                                {
                                        fac = (double) (System.Math.Sqrt(- 2.0 * System.Math.Log(r) / (double) (r)));
                                        normstore = v1 * fac;
                                        normdone = true;
                                        return v2 * fac;
                                }
                        }
                }
 
 
 
                /// <summary> Produces a random variate whose natural logarithm is from the
                /// Gaussian with mean=0 and the specified standard deviation.
                /// </summary>
                /// <param name="sigma">Standard deviation
                /// 
                /// </param>
                public virtual double LognormalDeviate(double sigma)
                {
                        return (double) (System.Math.Exp((double) (NormalDeviate() * sigma)));
                }
 
                /// <summary> Returns a uniformly distributed random real number between the specified
                /// inner and outer bounds.
                /// </summary>
                /// <param name="inner">Minimum value desired
                /// </param>
                /// <param name="outer">Maximum value desired
                /// 
                /// </param>
                public virtual double random_number(double inner, double outer)
                {
                        double range = outer - inner;
                        return (NextDouble() * range + inner);
                }
 
                /// <summary>   Returns a value within a certain uniform variation
                /// from the central value.
                /// </summary>
                /// <param name="value">Central value
                /// </param>
                /// <param name="variation">Maximum (uniform) variation above or below center
                /// 
                /// </param>
                public virtual double about(double value_Renamed, double variation)
                {
                        return (value_Renamed + (value_Renamed * random_number(- variation, variation)));
                }
 
        }
}
