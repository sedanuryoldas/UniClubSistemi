let savedEvents = JSON.parse(localStorage.getItem("savedEvents")) || [];

function saveEvent(id) {

    if (savedEvents.includes(id)) {
        alert("Already saved!");
        return;
    }

    savedEvents.push(id);
    localStorage.setItem("savedEvents", JSON.stringify(savedEvents));

    renderSaved();
    alert("Saved successfully!");
}

function renderSaved() {
    const savedContainer = document.getElementById("saved");

    if (!savedContainer) return;

    savedContainer.innerHTML = "";

    savedEvents.forEach(id => {
        savedContainer.innerHTML += `
      <div class="card p-3 mb-2">
        <strong>Event #${id}</strong>
        <button class="btn btn-danger mt-2"
                onclick="removeEvent(${id})">
          Remove ❌
        </button>
      </div>
    `;
    });
}

function removeEvent(id) {
    savedEvents = savedEvents.filter(e => e !== id);
    localStorage.setItem("savedEvents", JSON.stringify(savedEvents));

    renderSaved();
}

renderSaved();