async function main() {
    var con = new signalR.HubConnectionBuilder()
        .withUrl("/MemeHub")
        .build()

    con.on("Invalidate", () => {
        con.invoke("RequestUpdate")
    });

    con.on("Update", (memes) => {

        var root = document.getElementById("content");
        root.innerHTML = "";

        for (meme of memes) {
            var img = document.createElement("img");

            img.src = "/img/" + meme.name;

            img.width = 100;
            img.height = 100;
            img.alt = meme.name;

            img.style = "box-sizing: border-box; border: 5px solid white;"

            if (meme.active) {
                img.style = "box-sizing: border-box; border: 5px solid green;"
            }

            img.onclick = (element) => {
                con.invoke("PushUpdate", element.srcElement.alt);
            };

            root.appendChild(img);
        }
    });

    await con.start();

    await con.invoke("RequestUpdate");
}

main();