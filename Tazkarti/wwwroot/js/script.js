const cardContainer = document.querySelector(".container-cards");

if (cardContainer) {
  cardContainer.addEventListener("click", e => {
    if (e.target.tagName === "BUTTON") {
      const card = e.target.closest(".card");
      if (!card) return;

      const countSpan = card.querySelector("span");
      if (!countSpan) return;

      const text = countSpan.textContent;
      if (text.includes(":")) {
        let count = parseInt(text.split(":")[1].trim());
        if (!isNaN(count)) {
          count--;
          countSpan.textContent = `count :${count}`;

          if (count === 0) {
            alert("Tickets are out for this card");
          }
        }
      }
    }
  });
}
