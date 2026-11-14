using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class SeasonalRankTabs : MonoBehaviour
    {
        private static readonly string[] s_Kinds={"daily","weekly","seasonal","all"};
        [SerializeField] private Button[] m_Tabs;
        [SerializeField] private TMP_Text m_Timer;
        private int m_Index=2;
        private long m_End;
        private string m_Label;
        private long m_LastSecond=-1;
        internal event Action Selected;
        private void Awake()
        {
            for(int i=0;i<m_Tabs.Length;i++){int index=i;m_Tabs[i].onClick.AddListener(()=>Select(index));}
            Paint();
        }
        private void Select(int index)
        {
            m_Index=index;m_End=0;m_Label=null;
            Paint();Selected?.Invoke();
        }
        private void Paint()
        {
            for(int i=0;i<m_Tabs.Length;i++)
            {
                bool active=i==m_Index;
                m_Tabs[i].targetGraphic.color=active?new Color32(244,215,153,255):new Color32(64,72,111,255);
                var label=m_Tabs[i].GetComponentInChildren<TMP_Text>();
                label.color=active?new Color32(36,37,59,255):new Color32(250,242,221,255);
            }
            m_Timer.text=m_Index==3?"Lifetime trophies":"Loading standings…";
        }
        internal async Task<PeriodRankResponse> Fetch()
        {
            int index=m_Index;
            var ranks=SeasonalRanks.Current;
            if(ranks==null)throw new InvalidOperationException("Rank service unavailable.");
            var data=await ranks.Request(new Dictionary<string,object>{{"op","board"},{"kind",s_Kinds[index]}});
            if(this!=null&&m_Index==index){m_End=data.ends;m_Label=data.label;m_LastSecond=-1;Tick();}
            return data;
        }
        private void Update()=>Tick();
        private void Tick()
        {
            var ranks=SeasonalRanks.Current;
            if(ranks==null||m_End<=0||m_Index==3)return;
            long now=ranks.UtcSeconds;
            if(now==m_LastSecond)return;
            m_LastSecond=now;
            if(now>=m_End){m_End=0;m_Timer.text="Updating standings…";Selected?.Invoke();return;}
            var t=TimeSpan.FromSeconds(m_End-now);
            m_Timer.text=m_Label+"     •     Ends in "+(t.Days>0?$"{t.Days}d {t.Hours}h":$"{t.Hours:00}:{t.Minutes:00}:{t.Seconds:00}");
        }
    }
}
