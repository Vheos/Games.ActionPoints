inline void PremultiplyAlpha(inout fixed4 color)
{ color.rgb *= color.a; }

inline fixed4 PremultipliedAlpha(fixed4 color)
{ 
	color.rgb *= color.a; 
    return color;
}

inline bool Is01(fixed t)
{ return t >= 0 && t <= 1; }

inline bool Is01(fixed2 t)
{ 
	return t.x >= 0 && t.x <= 1 
	    && t.y >= 0 && t.y <= 1;
}

inline void ApplyST(inout fixed2 texcoord, fixed4 ST)
{ texcoord = texcoord * ST.xy + ST.zw; }

inline fixed2 AppliedST(fixed2 texcoord, fixed4 ST)
{ return texcoord.xy * ST.xy + ST.zw; }

inline fixed Random (fixed2 uv)
{ return frac(sin(dot(uv, fixed2(12.9898, 78.233))) * 43758.5453123); }

inline fixed Max(fixed a, fixed b, fixed c)
{ return max(a, max(b, c)); }

inline fixed Max(fixed a, fixed b, fixed c, fixed d)
{ return max(a, max(b, max(c, d))); }