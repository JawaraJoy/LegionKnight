using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class AetherHudTrails : MaskableGraphic
    {
        private static readonly Color s_Gold = new Color32(255, 226, 162, 255);
        private static readonly Color s_Violet = new Color32(162, 124, 255, 255);
        private static readonly Color s_Light = new Color32(239, 213, 255, 255);
        private bool m_Ready, m_Active;
        private float m_Phase, m_EdgeOpacity;

        public void Present(bool ready, bool active, float phase, float edgeOpacity)
        {
            bool redraw = ready || active || edgeOpacity > 0f || m_Ready || m_Active || m_EdgeOpacity > 0f;
            if (!redraw || (ready == m_Ready && active == m_Active && phase == m_Phase && edgeOpacity == m_EdgeOpacity)) return;
            m_Ready = ready;
            m_Active = active;
            m_Phase = phase;
            m_EdgeOpacity = edgeOpacity;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect bounds = rectTransform.rect;
            Vector2 crest = new Vector2(bounds.center.x, bounds.yMin + 83f);
            if (m_Ready) DrawReady(mesh, crest);
            if (m_EdgeOpacity > 0f) DrawRails(mesh, bounds);
            if (m_Active && m_Phase < .74f) DrawActivation(mesh, bounds, crest);
        }

        private void DrawReady(VertexHelper mesh, Vector2 crest)
        {
            for (int i = 0; i < 2; i++)
            {
                float t = (m_Phase - i * .095f) / .55f;
                if (t < 0f || t >= 1f) continue;
                float radius = Mathf.Lerp(24, 69, Mathf.SmoothStep(0, 1, t));
                Ring(mesh, crest, radius, radius * .56f, 2f, Tint(s_Gold, .83f * (1 - t)));
            }
            for (int i = 0; i < 14; i++)
            {
                float t = (m_Phase - (i % 3) * .045f) / .81f;
                if (t <= 0 || t >= 1) continue;
                Vector2 dir = Direction(i / 14f - .032f);
                Vector2 p = crest + Vector2.Scale(dir, new Vector2(24 + 84*t, 16 + 59*t)) + Vector2.up * (9*t);
                Star(mesh, p, 4.5f - 3*t, Tint(s_Gold, Mathf.Sin(t * Mathf.PI)));
            }
            for (int i = 0; i < 6; i++)
            {
                float t = Mathf.Repeat(m_Phase*.55f + i*.371f, 1);
                Vector2 p = crest + Vector2.Scale(Direction(i/6f + .02f), new Vector2(53 + 9*t, 26 + 12*t));
                Star(mesh, p, 2.5f, Tint(s_Gold, Mathf.Sin(t*Mathf.PI)*.85f));
            }
        }

        private void DrawRails(VertexHelper mesh, Rect r)
        {
            float opacity = m_EdgeOpacity * (.86f + .12f*Mathf.Sin(m_Phase*3f));
            for (int side = 0; side < 2; side++)
            {
                float edge = side == 0 ? r.xMin : r.xMax;
                float inward = side == 0 ? 1f : -1f;
                for (int strip = 0; strip < 5; strip++)
                {
                    float t = strip/5f, next = (strip+1)/5f;
                    float x0 = edge + inward * 22*t, x1 = edge + inward * 22*next;
                    Color a = Tint(s_Violet, .54f*opacity*(1-t)*(1-t));
                    Color b = Tint(s_Violet, .54f*opacity*(1-next)*(1-next));
                    Quad(mesh, new Vector2(x0,r.yMin),new Vector2(x0,r.yMax),new Vector2(x1,r.yMax),new Vector2(x1,r.yMin),a,a,b,b);
                }
                float span = r.height + 94;
                int count = Mathf.CeilToInt(span/162f);
                for (int i = 0; i < count; i++)
                {
                    float y = r.yMin-65 + Mathf.Repeat(i*span/count + m_Phase*102f + side*28f, span);
                    Vector2 a = new Vector2(edge+inward*3, y);
                    Vector2 b = a + Vector2.up*22;
                    Vector2 c = b + new Vector2(inward*5,15);
                    Vector2 head = c + Vector2.up*13;
                    Color tint = Tint(s_Light, m_EdgeOpacity*.83f);
                    Line(mesh,a,b,1.8f,tint); Line(mesh,b,c,1.8f,tint); Line(mesh,c,head,1.8f,tint);
                    Star(mesh,head,3f,tint);
                }
            }
        }

        private void DrawActivation(VertexHelper mesh, Rect bounds, Vector2 crest)
        {
            float t = Mathf.Clamp01(m_Phase / .73f);
            float radius = 25 + bounds.size.magnitude * Mathf.Pow(t,.86f);
            Ring(mesh, crest, radius, radius, 2.6f, Tint(s_Light,.82f*(1-t)));
            float head = Mathf.Clamp01(m_Phase / .43f);
            for (int side = -1; side <= 1; side += 2)
            {
                Vector2 previous = crest;
                for (int i = 0; i <= 14; i++)
                {
                    float v = Mathf.Lerp(Mathf.Max(0,head-.24f),head,i/14f);
                    Vector2 point = crest + new Vector2(side*(bounds.width*.5f-15)*Mathf.Min(1,v*2.65f),v*bounds.height*.78f);
                    if (i > 0) Line(mesh,previous,point,3,Tint(s_Light,.8f*(1-t)));
                    previous=point;
                }
            }
        }

        private static Color Tint(Color color,float opacity) { color.a = Mathf.Clamp01(opacity); return color; }
        private static Vector2 Direction(float turns)
        {
            float a = turns * Mathf.PI * 2;
            return new Vector2(Mathf.Cos(a),Mathf.Sin(a));
        }
        private static void Ring(VertexHelper mesh, Vector2 center, float rx, float ry, float width, Color color)
        {
            Vector2 size = new Vector2(rx,ry);
            Vector2 previous = center + Vector2.right*rx;
            for(int i=1;i<=72;i++)
            {
                Vector2 next = center + Vector2.Scale(Direction(i/72f),size);
                Line(mesh,previous,next,width,color);
                previous=next;
            }
        }
        private static void Star(VertexHelper mesh, Vector2 center,float radius,Color color)
        {
            Quad(mesh,center+Vector2.up*radius,center+Vector2.right*(radius*.30f),center-Vector2.up*radius,center-Vector2.right*(radius*.30f),color,color,color,color);
            Quad(mesh,center+Vector2.up*(radius*.30f),center+Vector2.right*radius,center-Vector2.up*(radius*.30f),center-Vector2.right*radius,color,color,color,color);
        }
        private static void Line(VertexHelper mesh, Vector2 a, Vector2 b,float width,Color color)
        {
            Vector2 axis = (b-a).normalized;
            Vector2 normal = new Vector2(-axis.y,axis.x)*width*.5f;
            Quad(mesh,a-normal,a+normal,b+normal,b-normal,color,color,color,color);
        }
        private static void Quad(VertexHelper mesh, Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color ca,Color cb,Color cc,Color cd)
        {
            int i=mesh.currentVertCount;
            mesh.AddVert(a,ca,Vector2.zero); mesh.AddVert(b,cb,Vector2.zero);
            mesh.AddVert(c,cc,Vector2.zero); mesh.AddVert(d,cd,Vector2.zero);
            mesh.AddTriangle(i,i+1,i+2); mesh.AddTriangle(i,i+2,i+3);
        }
    }
}
