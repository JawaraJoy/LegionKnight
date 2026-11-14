const axios = require("axios-0.21");
const DAY = 86400000;
const EPOCH = Date.UTC(2026, 10, 14);
const boards = ["daily", "weekly", "seasonal"];
const amounts = [100, 70, 50, 30, 15, 5];
const factors = { daily: 1, weekly: 5, seasonal: 30 };
const pageSize = 16;
function startOf(kind, when) {
    const midnight = Math.floor(when / DAY) * DAY;
    if (kind === "daily") return midnight;
    if (kind === "weekly") return midnight - ((new Date(midnight).getUTCDay() + 6) % 7) * DAY;
    return EPOCH + Math.floor((when - EPOCH) / (90 * DAY)) * 90 * DAY;
}
function label(kind, start) {
    if (kind === "seasonal") return "Season " + String(1 + Math.floor((start - EPOCH) / (90 * DAY))).padStart(2, "0");
    return (kind === "daily" ? "Daily · " : "Week of ") + new Date(start).toISOString().slice(0, 10);
}
function diamonds(kind, rank) {
    return factors[kind] * amounts[rank === 1 ? 0 : rank <= 3 ? 1 : rank <= 10 ? 2 : rank <= 30 ? 3 : rank <= 100 ? 4 : 5];
}
function num(v) { return Number.isSafeInteger(v) && v >= 0; }
function status(e) { return e.response && e.response.status; }
function checkedTotal(parts) {
    const sum = Object.values(parts).reduce((a,b) => a+b, 0);
    if (!num(sum) || sum > 2147483647) throw Error("Trophy total out of range");
    return sum;
}
module.exports = async ({params, context}) => {
    const {projectId, playerId, serviceToken, accessToken} = context;
    if (!playerId) throw Error("Player sign-in required");
    const cloud = axios.create({baseURL:"https://cloud-save.services.api.unity.com/v1/data/projects/"+projectId+"/players/"+playerId, headers:{Authorization:"Bearer "+serviceToken},timeout:5000});
    const lb = axios.create({baseURL:"https://leaderboards.services.api.unity.com/v1/projects/"+projectId+"/leaderboards/",headers:{Authorization:"Bearer "+accessToken},timeout:5000});
    const boardId = k => k === "all" ? "aj_rank_all_best" : "aj_rank_"+k;
    const base = (k,v) => boardId(k)+(v?"/versions/"+v:"")+"/scores";
    const now = Date.now();
    const monthOf = t=>new Date(t).toISOString().slice(0,7).replace("-","");
    const mailKey = id => "rankv1_mail_"+id.split("_")[0]+"_"+id.split("_")[1].slice(0,6);
    async function read(keys, after, path="/protected") {
        const fields=(keys||[]).map(k=>"keys="+encodeURIComponent(k));
        if(after)fields.push("after="+encodeURIComponent(after));
        return (await cloud.get(path+"/items"+(fields.length?"?"+fields.join("&"):""))).data;
    }
    const indexed = rows => Object.fromEntries(rows.results.map(x=>[x.key,x]));
    const write = (key,value,previous) => previous ? {key,value,writeLock:previous.writeLock} : {key,value};
    async function batch(data, path="/protected") { await cloud.post(path+"/item-batch",{data}); }
    // Serialise first writes too: a missing per-reward key cannot supply its own write lock.
    const fenceKey = "rankv1_fence";
    async function change(keys,compute,path="/protected") {
        for(let i=0;i<6;i++) {
            const rows=indexed(await read([...new Set(keys.concat(fenceKey))],null,path));
            if(!rows[fenceKey]) { await batch([{key:fenceKey,value:0}],path);continue; }
            const out=compute(rows);
            if(!out.writes.length) return out.value;
            try { await batch(out.writes.concat(write(fenceKey,rows[fenceKey].value+1,rows[fenceKey])),path);return out.value; }
            catch(e) { if(status(e)!==409||i===5) throw e; }
        }
        throw Error("Please retry");
    }
    async function own(k,v) {
        try{return (await lb.get(base(k,v)+"/players/"+playerId)).data;}
        catch(e){if(status(e)===404)return null;throw e;}
    }
    const listScores=async (k,v,offset,limit)=>(await lb.get(base(k,v),{params:{offset,limit}})).data.results;
    async function tiedRank(k,v,entry) {
        if(!entry || entry.rank===0) return entry?1:0;
        // Find the first equal score, including ties outside the visible page.
        let lo=0, hi=entry.rank;
        const previous=await listScores(k,v,hi-1,1);
        if(previous[0] && previous[0].score>entry.score)return hi+1;
        while(lo<hi){const mid=Math.floor((lo+hi)/2);const page=await listScores(k,v,mid,1);
            if(!page[0])throw Error("Rank snapshot changed");if(page[0].score>entry.score)lo=mid+1;else hi=mid;}
        return lo+1;
    }
    const version = async k=>(await lb.get(boardId(k)+"/versions",{params:{limit:1}})).data;
    const seconds=t=>Math.floor(t/1000);
    const response={serverTime:seconds(now)};
    if(params.op==="board") {
        const k=params.kind;if(!boards.concat("all").includes(k))throw Error("Unknown period");
        const [v,page,me]=await Promise.all([k==="all"?null:version(k),listScores(k,null,0,30),own(k,null)]);
        let rank=0,prev=null;
        const rows=page.map((e,i)=>{if(e.score!==prev)rank=i+1;prev=e.score;return {id:e.playerId,name:e.playerName,score:e.score,rank};});
        const self=rows.find(r=>r.id===playerId);
        return {...response,rows,own:self||(me?{id:playerId,name:me.playerName,score:me.score,rank:await tiedRank(k,null,me)}:null),
            ends:v?seconds(Date.parse(v.nextReset)):0,label:k==="all"?"All Time":label(k,startOf(k,now))};
    }
    // Previous clients only know their cumulative total. Acknowledge their queue without ranking it.
    if(params.op==="score") return response;
    if(params.op==="runBest") {
        const t=params.day*DAY, best=params.total;
        if(!num(params.day)||!num(t)||t<EPOCH||t>startOf("daily",now)||!num(best)||best<1||best>2147483647)
            throw Error("Invalid Colossal run score");
        const versions=await Promise.all(boards.map(version));
        const active=boards.map((k,i)=>({k,v:versions[i],t:startOf(k,t)})).filter(p=>p.t===startOf(p.k,now));
        if(active.some(p=>Date.parse(p.v.nextReset)<=Date.now()+8000))throw Error("Leaderboard rollover; retry");

        // All four boards are descending keepBest. Replays, lower runs, and scores from another
        // device can never add to or replace a better score.
        await Promise.all(["all",...active.map(p=>p.k)].map(k=>
            lb.post(base(k,null)+"/players/"+playerId,{score:best})));
        if(active.length) {
            const key="rankbest_open_"+monthOf(t);
            await change([key],rows=>{
                const open={...(rows[key]?rows[key].value:{})};
                let changed=false;
                for(const p of active) {
                    const id=p.k+"_"+p.v.versionId;
                    if(open[id])continue;
                    open[id]={kind:p.k,version:p.v.versionId,start:p.t,end:Date.parse(p.v.nextReset)};
                    changed=true;
                }
                return {value:null,writes:changed?[write(key,open,rows[key])]:[]};
            });
        }
        return response;
    }
    if(params.op==="mail") {
        // Discover participation through monthly pages. Only actual participants are looked up in archives.
        const startMonth=params.cursor || new Date(now).toISOString().slice(0,7);
        if(!/^\d{4}-\d{2}$/.test(startMonth))throw Error("Invalid cursor");
        const months=[];let at=Date.parse(startMonth+"-01T00:00:00Z");const epochMonth=Date.UTC(2026,10,1);
        for(let i=0;i<6&&at>=epochMonth;i++){months.push(new Date(at).toISOString().slice(0,7).replace("-",""));const d=new Date(at);at=Date.UTC(d.getUTCFullYear(),d.getUTCMonth()-1,1);}
        const keys=months.map(m=>"rankbest_open_"+m);
        const records=keys.length?indexed(await read(keys)):{};
        const pending=[];for(const key of keys)for(const [id,p] of Object.entries(records[key]?records[key].value:{})){if(p.end<=now)pending.push({id,p,key});}
        const sent=params.after||"";const unique=Array.from(new Map(pending.map(p=>[p.id,p])).values());
        const ordered=unique.sort((a,b)=>a.id.localeCompare(b.id)).filter(x=>x.id>sent);
        const selected=ordered.slice(0,pageSize);
        const selectedKeys=[...new Set(selected.map(x=>mailKey(x.id)))];
        const stored=selectedKeys.length?indexed(await read(selectedKeys)):{};
        const mail=await Promise.all(selected.map(async ({id,p})=>{
            const stateKey=mailKey(id);
            if(stored[stateKey] && stored[stateKey].value[id])return stored[stateKey].value[id];
            const e=await own(p.kind,p.version);if(!e||e.score<=0)return null;
            const rank=await tiedRank(p.kind,p.version,e);
            const rec={id,kind:p.kind,label:label(p.kind,p.start),rank,score:e.score,amount:diamonds(p.kind,rank),state:"pending"};
            return rec;
        }));
        const fresh=mail.filter(m=>m && !(stored[mailKey(m.id)] && stored[mailKey(m.id)].value[m.id]));
        if(fresh.length){
            const groupKeys=[...new Set(fresh.map(m=>mailKey(m.id)))];
            await change(groupKeys,r=>{
                const groups={};
                for(const m of fresh){const key=mailKey(m.id);
                    if(!groups[key])groups[key]={...(r[key]?r[key].value:{})};
                    if(!groups[key][m.id])groups[key][m.id]=m;
                }
                return {value:null,writes:groupKeys.map(k=>write(k,groups[k],r[k]))};
            });
        }
        return {...response,mail:mail.filter(Boolean),cursor:ordered.length>pageSize?startMonth:(at>=epochMonth?new Date(at).toISOString().slice(0,7):""),after:ordered.length>pageSize?selected[selected.length-1].id:""};
    }
    if(params.op==="send"||params.op==="claim") {
        const id=params.id;
        if(!/^(daily|weekly|seasonal)_[a-zA-Z0-9-]{1,50}$/.test(id))throw Error("Invalid reward ID");
        const key=mailKey(id);
        if(params.op==="claim"){
            const protectedRows=indexed(await read([key]));
            const rec=protectedRows[key] && protectedRows[key].value[id];
            if(!rec || rec.state==="pending")throw Error("Send the reward to Inbox first");
            if(rec.state!=="claimed"){
                // Receipt and wallet share the same atomic batch and conflict fence as purchases.
                const balance="iap_diamonds_v2", walletFence="iap_purchase_fence_v1";
                const receipt="rank_credit_"+rec.kind+"_"+String(rec.id.split("_")[1]).slice(0,6);
                await change([receipt,balance,walletFence], r=>{
                    const seen={...(r[receipt]?r[receipt].value:{})};
                    if(seen[id])return {value:null,writes:[]};
                    seen[id]=true;
                    const next=checkedTotal({balance:r[balance]?r[balance].value:0,credit:rec.amount});
                    return {value:null,writes:[write(receipt,seen,r[receipt]),write(balance,next,r[balance]),write(walletFence,id,r[walletFence])]};
                },"");
            }
        }
        const value=await change([key],r=>{
            if(!r[key]||!r[key].value[id])throw Error("Reward not found");
            const month={...r[key].value}, m={...month[id]};
            if(params.op==="send" && m.state==="pending")m.state="inbox";
            if(params.op==="claim" && m.state==="inbox")m.state="claimed";
            if(m.state===month[id].state)return {value:m,writes:[]};
            month[id]=m;return {value:m,writes:[write(key,month,r[key])]};
        });return {...response,mail:[value]};
    }
    throw Error("Unknown Seasonal Rank operation");
};
module.exports.params = {
  "op": {
    "type": "String",
    "required": true
  },
  "kind": "String",
  "day": "Numeric",
  "device": "String",
  "total": "Numeric",
  "legacy": "Numeric",
  "cursor": "String",
  "after": "String",
  "id": "String"
};
