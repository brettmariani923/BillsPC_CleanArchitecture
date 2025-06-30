const battleOver = "@Model.BattleOver.ToString().ToLower()" === "true";
const isPlayerTurn = "@Model.IsPlayerTurn.ToString().ToLower()" === "true";

let musicWindow = null;

function playAttackSound(moveName) {
    if (!moveName) return;

    const encodedName = encodeURIComponent(moveName.trim());
    const soundSrc = `/sounds/${encodedName}.mp3`;

    const audio = new Audio(soundSrc);
    audio.volume = 0.5;

    audio.onerror = () => {
        const fallback = new Audio("/sounds/tackle.mp3");
        fallback.volume = 0.5;
        fallback.play().catch(e => console.warn("Fallback audio failed:", e));
    };

    audio.play().catch(e => console.warn("Audio play failed:", e));
}

function updateHPBar(id) {
    const bar = document.getElementById(id);
    if (!bar) return;

    const max = parseInt(bar.dataset.max);
    const current = parseInt(bar.dataset.current);
    const next = parseInt(bar.dataset.new);

    const percent = Math.max(0, (next / max) * 100);

    setTimeout(() => {
        bar.style.width = percent + '%';

        bar.classList.remove('low', 'medium', 'high');
        if (percent <= 25) bar.classList.add('low');
        else if (percent <= 60) bar.classList.add('medium');
        else bar.classList.add('high');
    }, 50);
}

function openMusicTab(songFile = "Battle!.mp3") {
    if (musicWindow && !musicWindow.closed) {
        musicWindow.focus();
        // Do NOT rewrite document or reset audio
        return;
    }

    musicWindow = window.open("", "MusicPlayer", "width=300,height=100");

    if (musicWindow) {
        musicWindow.document.write(`
          <html>
          <head><title>Music Player</title></head>
          <body style="margin:0; display:flex; flex-direction:column; align-items:center; justify-content:center; height:100%;">
            <audio id="bgm" loop controls style="width:100%;">
              <source src="/sounds/${encodeURIComponent(songFile)}" type="audio/mpeg" />
              Your browser does not support the audio element.
            </audio>
            <button id="playButton" style="display:none; margin-top:10px;">Play Music</button>

            <script>
              const audio = document.getElementById('bgm');
              const playButton = document.getElementById('playButton');
              audio.volume = 0.5;

              function tryPlay() {
                audio.play().catch(err => {
                  console.warn("Autoplay prevented:", err);
                  playButton.style.display = "block";
                });
              }

              playButton.onclick = () => {
                audio.play();
                playButton.style.display = "none";
              };

              tryPlay();
            <\/script>
          </body>
          </html>
        `);
    }
}


function performAttack(attackerId, targetId, moveName) {
    if (battleOver) return;

    const attacker = document.getElementById(attackerId);
    const target = document.getElementById(targetId);

    if (!attacker || !target) return;

    playAttackSound(moveName);

    attacker.classList.add('attack-forward');
    setTimeout(() => {
        attacker.classList.remove('attack-forward');
        attacker.classList.add('attack-back');
        target.classList.add('shake', 'flash');

        if (targetId === 'pokemon2-sprite') {
            updateHPBar('ai-hp-bar');
        } else if (targetId === 'pokemon1-sprite') {
            updateHPBar('player-hp-bar');
        }

        setTimeout(() => {
            target.classList.remove('shake', 'flash');
            attacker.classList.remove('attack-back');
        }, 500);
    }, 300);
}

window.addEventListener('DOMContentLoaded', () => {
    if (battleOver) return;

    updateHPBar('player-hp-bar');
    updateHPBar('ai-hp-bar');

    if (isPlayerTurn) {
        document.querySelectorAll('#moveForm button.move-button').forEach(button => {
            button.addEventListener('click', function () {
                const move = this.getAttribute('data-move');
                if (!move) return;

                document.getElementById('moveNameInput').value = move;

                // Animate HP bar reduction for opponent
                const aiHpBar = document.getElementById('ai-hp-bar');
                if (aiHpBar) {
                    const currentHP = parseInt(aiHpBar.dataset.new);
                    const newHP = Math.max(0, currentHP - 10);  // Adjust '10' to match expected damage animation
                    aiHpBar.dataset.new = newHP;
                    updateHPBar('ai-hp-bar');
                }

                performAttack('pokemon1-sprite', 'pokemon2-sprite', move);

                setTimeout(() => {
                    document.getElementById('moveForm').submit();
                }, 800);
            });
        });
    } else {
        setTimeout(() => {
            const log = document.getElementById('battleText')?.textContent || "";
            const match = log.match(/used\s+(.*?)\s+and dealt/i);
            const aiMoveName = match?.[1]?.trim() || "default";

            performAttack('pokemon2-sprite', 'pokemon1-sprite', aiMoveName);
        }, 100);
    }

 

});
