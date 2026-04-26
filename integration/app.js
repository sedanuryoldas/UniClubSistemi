const events = [
  { id: 1, title: "AI Workshop", date: "2026-05-01" },
  { id: 2, title: "Music Festival", date: "2026-05-03" }
];

const container = document.getElementById("events");

events.forEach(event => {
  const div = document.createElement("div");
  div.innerHTML = `<h3>${event.title}</h3><p>${event.date}</p>`;
  container.appendChild(div);
});