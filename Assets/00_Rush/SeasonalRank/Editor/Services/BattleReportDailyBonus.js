const axios = require("axios-0.21");

const REWARD = 10;
const DAY_MS = 86400000;
const LAST_DAY = "battle_share_last_utc_day_v1";
const BALANCE = "iap_diamonds_v2";
const FENCE = "iap_purchase_fence_v1";

module.exports = async ({ params, context }) => {
    const { projectId, playerId, serviceToken } = context;
    if (!playerId) throw Error("Player sign-in required.");
    if (params.action !== "status" && params.action !== "claim") throw Error("Unknown action.");

    const cloud = axios.create({
        baseURL: "https://cloud-save.services.api.unity.com/v1/data/projects/" + projectId + "/players/" + playerId,
        headers: { Authorization: "Bearer " + serviceToken },
        timeout: 5000
    });
    const claiming = params.action === "claim";
    const keys = claiming ? [LAST_DAY, BALANCE, FENCE] : [LAST_DAY];
    const endpoint = "/items?" + keys.map(key => "keys=" + key).join("&");
    // Assign this tap its UTC day once, including all write-lock retries.
    const now = Date.now();
    const day = Math.floor(now / DAY_MS);
    const result = (claimed, granted, expired = false) => ({
        claimed, granted, expired, reward: REWARD, serverTime: Math.floor(now / 1000),
        resetAt: (day + 1) * (DAY_MS / 1000)
    });
    for (let attempt = 0; attempt < 8; attempt++) {
        const response = await cloud.get(endpoint);
        const rows = Object.fromEntries(response.data.results.map(row => [row.key, row]));
        const claimed = !!rows[LAST_DAY] && rows[LAST_DAY].value >= day;
        if (!claiming) return result(claimed, 0);
        if (!Number.isSafeInteger(params.launchDay) || params.launchDay !== day) return result(claimed, 0, true);
        if (claimed) return result(true, 0);
        // Establish the same fence used by account Diamond debits and credits, then read its lock.
        if (!rows[FENCE]) {
            await cloud.post("/item-batch", {data: [{key: FENCE, value: "battle-share-init"}]});
            continue;
        }
        const balance = rows[BALANCE] ? rows[BALANCE].value : 0;
        if (!Number.isSafeInteger(balance) || balance < 0 || balance > 2147483647 - REWARD)
            throw Error("Invalid Diamond balance.");
        const entry = (key, value) => rows[key] ? {key, value, writeLock: rows[key].writeLock} : {key, value};
        try {
            await cloud.post("/item-batch", {data: [
                entry(LAST_DAY, day), entry(BALANCE, balance + REWARD), entry(FENCE, "battle-share-" + day)
            ]});
            return result(true, REWARD);
        } catch (err) {
            if (!err.response || err.response.status !== 409) throw err;
        }
    }
    throw Error("Wallet busy. Try again.");
};
module.exports.params = { action: {type: "String", required: true}, launchDay: {type: "Numeric"} };
