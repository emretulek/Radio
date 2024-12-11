﻿using System.Windows.Media;

namespace Radio
{
    public static class IconPaths
    {
        private static readonly string PlayPath = "M 6.1057828,63.999624 A 6.1512566,6.1512566 0 0 1 3.0623268,63.187481 C 0.97539178,62.004946 -0.32200322,59.70923 -0.32200322,57.217092 V 6.7826921 c 0,-2.499092 1.297432,-4.787778 3.38433002,-5.97038902 a 6.1095183,6.1095183 0 0 1 6.22082,0.07861 L 52.387174,26.692465 a 6.2608214,6.2608214 0 0 1 0,10.608612 L 9.2761928,63.109548 a 6.1738653,6.1738653 0 0 1 -3.17041,0.890454 z";
        private static readonly string PausePath = "M 7.6906505,2.9120095 C 7.1110983,4.0497216 7.1110983,5.5431249 7.1110983,8.5333333 V 55.466629 c 0,2.986655 0,4.48002 0.5795522,5.621323 a 5.3333268,5.3333268 0 0 0 2.3324575,2.328905 C 11.160858,64 12.654223,64 15.644432,64 h 0.711117 c 2.986655,0 4.479983,0 5.621324,-0.57959 a 5.3333268,5.3333268 0 0 0 2.332419,-2.33242 c 0.57959,-1.137712 0.57959,-2.631116 0.57959,-5.621324 V 8.533371 c 0,-2.9866552 0,-4.4800206 -0.57959,-5.6213237 A 5.3333268,5.3333268 0 0 0 21.976873,0.57958976 C 20.839085,-1.875e-7 19.345757,-1.875e-7 16.355549,-1.875e-7 h -0.711117 c -2.986656,0 -4.480021,0 -5.621324,0.5795899475 A 5.3333268,5.3333268 0 0 0 7.6906505,2.9120095 Z m 31.9999825,0 c -0.57959,1.1377121 -0.57959,2.6311154 -0.57959,5.6213238 V 55.466629 c 0,2.986655 0,4.48002 0.583142,5.621323 a 5.3333268,5.3333268 0 0 0 2.328905,2.328905 C 43.160802,64 44.654168,64 47.644414,64 h 0.711155 c 2.986655,0 4.479983,0 5.621323,-0.57959 a 5.3333268,5.3333268 0 0 0 2.328867,-2.33242 c 0.583143,-1.137712 0.583143,-2.631116 0.583143,-5.621324 V 8.533371 c 0,-2.9866552 0,-4.4800206 -0.57959,-5.6213237 A 5.3333268,5.3333268 0 0 0 53.976892,0.57958976 C 52.839029,3.7607738e-5 51.345777,3.7607738e-5 48.355569,3.7607738e-5 h -0.711155 c -2.986656,0 -4.479983,0 -5.621324,0.579552152262 A 5.3333268,5.3333268 0 0 0 39.694185,2.9120473 Z";
        private static readonly string StopPath = "M -4.6743125e-7,55.322732 C -4.6743125e-7,60.780371 3.2982428,64 8.8344619,64 H 55.165575 c 5.536215,0 8.834458,-3.219629 8.834458,-8.677268 V 8.6773065 C 64.000033,3.2196673 60.70179,4.6743145e-7 55.165575,4.6743145e-7 H 8.8344619 C 3.2982466,4.6743145e-7 -4.6743125e-7,3.2197051 -4.6743125e-7,8.6773065 Z";
        private static readonly string ForwardPath = "M 56.00125,-3.1715222e-5 A 2.6668035,2.6668035 0 0 0 53.33443,2.6667883 V 25.406273 L 14.537441,2.1851013 A 5.8553003,5.8553003 0 0 0 8.575459,2.1121513 C 6.575344,3.2450853 5.331967,5.4384953 5.331967,7.8336293 V 56.169463 c 0,2.395096 1.243339,4.588544 3.243492,5.721931 a 5.8569672,5.8569672 0 0 0 5.961982,-0.07559 L 53.33443,38.59701 v 22.739484 a 2.6668035,2.6668035 0 0 0 5.333603,0 V 2.6667883 A 2.6668035,2.6668035 0 0 0 56.00125,-3.1715222e-5 Z";
        private static readonly string BackwardPath = "M 7.9987735,-1.374354e-4 A 2.666805,2.666805 0 0 1 10.665558,2.6666466 V 25.406183 L 49.462569,2.1849586 a 5.8553038,5.8553038 0 0 1 5.961985,-0.07295 c 2.000116,1.132935 3.243494,3.333074 3.243494,5.721557 V 56.169391 c 0,2.395135 -1.24334,4.588585 -3.243494,5.721973 a 5.8569707,5.8569707 0 0 1 -5.961985,-0.0756 L 10.665558,38.59689 v 22.739498 a 2.666805,2.666805 0 0 1 -5.3336065,0 V 2.6666846 A 2.666805,2.666805 0 0 1 7.9987735,-9.9435399e-5 Z"; 
        private static readonly string SoundHighPath = "M 54.24702,51.514546 49.249451,47.516368 C 60.389048,35.34333 60.389048,16.734448 49.249301,4.5614426 l 4.997527,-3.9979844 c 13.0043,14.5207958 13.004108,36.4302908 1.5e-4,50.9510878 z m -12.670155,-40.814983 -5.00343,4.00266 c 5.975084,6.391292 5.974892,16.282234 1.5e-4,22.673525 l 5.00343,4.00266 c 7.838187,-8.74158 7.838187,-21.937456 -1.5e-4,-30.678845 z M 28.79015,0.22367581 12.146035,13.32064 H 0 V 38.945255 H 12.164805 L 28.79015,51.776332 Z M 6.4061536,19.726793 h 7.9607674 l 8.017077,-6.309195 V 38.738786 L 14.348151,32.539102 H 6.4061536 Z";
        private static readonly string SoundLowPath = "M 41.937839,41.511882 36.890992,37.474487 C 42.917536,31.027736 42.91773,21.050968 36.89084,14.604216 l 5.046805,-4.037426 c 7.906395,8.817243 7.906395,22.127624 1.51e-4,30.945092 z M 29.039974,0 12.251432,13.210612 H 0 v 25.84697 H 12.270364 L 29.039974,52 Z M 6.4617424,19.672354 h 8.0298466 l 8.086645,-6.363943 v 25.54091 L 14.472655,32.59584 H 6.4617424 Z";
        private static readonly string SoundMutePath = "M 54.926931,26.181819 64,35.254888 l -4.593629,4.593629 -9.073069,-9.073069 -9.073069,9.073069 -4.593627,-4.593629 9.073069,-9.073069 -9.073069,-9.073069 4.593627,-4.593627 9.073069,9.073069 9.073069,-9.073069 L 64,17.10875 Z M 29.198769,0 12.318424,13.28285 H 0 V 39.271153 H 12.33746 L 29.198769,52.284342 Z M 6.4970761,19.779925 H 14.57083 l 8.130864,-6.398742 v 25.68057 L 14.551793,32.774078 H 6.4970761 Z";
        private static readonly string NotaSinglePath = "M 101.33169,47.896905 v 26.66441 18.81441 A 13.333162,13.333162 0 0 0 93.334706,90.668559 13.333162,13.333162 0 0 0 80.001525,104.00174 13.333162,13.333162 0 0 0 93.334706,117.33492 13.333162,13.333162 0 0 0 106.66789,104.00174 V 71.893703 L 128,61.229599 Z";
        private static readonly string NotaDualPath = "M 80.001037,0 21.331624,10.652384 V 26.77916 h -5e-4 V 61.334097 A 13.333162,13.333162 0 0 0 13.333181,58.667459 13.333162,13.333162 0 0 0 0,72.000641 13.333162,13.333162 0 0 0 13.333181,85.333822 13.333162,13.333162 0 0 0 26.666362,72.000641 a 13.333162,13.333162 0 0 0 -0.0015,-0.05957 V 31.172933 L 74.6648,22.457657 v 33.53926 c -2.228098,-1.673233 -4.9969,-2.66517 -7.997954,-2.66517 -7.363365,0 -13.334646,5.972744 -13.334646,13.336109 0,7.363365 5.971281,13.332206 13.334646,13.332206 7.365365,0 13.334156,-5.968841 13.334156,-13.332206 V 21.488859 Z";

        public static Geometry Play() { return Geometry.Parse(PlayPath); }
        public static Geometry Pause() { return Geometry.Parse(PausePath); }
        public static Geometry Stop() { return Geometry.Parse(StopPath); }
        public static Geometry Forward() { return Geometry.Parse(ForwardPath); }
        public static Geometry Backward() { return Geometry.Parse(BackwardPath); }
        public static Geometry SoundHigh() { return Geometry.Parse(SoundHighPath); }
        public static Geometry SoundLow() { return Geometry.Parse(SoundLowPath); }
        public static Geometry SoundMute() { return Geometry.Parse(SoundMutePath); }
        public static Geometry NotaSingle() { return Geometry.Parse(NotaSinglePath); }
        public static Geometry NotaDual() { return Geometry.Parse(NotaDualPath); }
    }
}