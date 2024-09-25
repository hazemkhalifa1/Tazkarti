const cardContainer = document.querySelector(".container-cards");

cardContainer.addEventListener("click", e => {
  if (e.target.tagName === "BUTTON") {
    const card = e.target.closest(".card");

    const countSpan = card.querySelector("span");
    let count = parseInt(countSpan.textContent.split(":")[1].trim());

    count--;

    countSpan.textContent = `count :${count}`;

    if (count === 0) {
      alert("Tickets are out for this card");
    }
  }
});
