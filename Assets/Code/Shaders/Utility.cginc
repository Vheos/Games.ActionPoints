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