const win = document.getElementById("chat-window");
const form = document.getElementById("chat-form");
const input = document.getElementById("chat-input");

const SPEAKER_CLASS = {
    "Achilles": "achilles",
    "Patroclus": "patroclus",
    "Both": "both"
};

function addMessage(role, text, meta) {
    const row = document.createElement("div");
    row.className = `msg ${ role }`;
    const bubble = document.createElement("div");
    bubble.className = "bubble";
    bubble.textContent = text;
    const sub = document.createElement("div");
    sub.className = "meta";
    sub.textContent = meta || "";
    row.appendChild(bubble);
    row.appendChild(sub);
    win.appendChild(row);
    win.scrollTop = win.scrollHeight;
}

form.addEventListener("submit", async (e) => {
    e.preventDefault();
    const msg = input.value.trim();
    if (!msg) return;

    addMessage("user", msg, "You");
    input.value = "";

    try {
        const res = await fetch("/api/chat", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ message: msg })
        });
        const data = await res.json();

        const roleClass = SPEAKER_CLASS[data.speaker] ?? "both";
        const meta = `${ data.speaker } — ${ data.mode }`;
        addMessage(roleClass, data.text, meta);

    } catch (err) {
        addMessage("both", "Something went wrong. Please try again.", "Error");
    }
});