//Call API
const loginAPI = "";
fetch(loginAPI)
    .then(function (response) {
        return response.json();
        //JSON --> Javascript type
    })
    .then(function (posts) {
        var htmls = posts.map(function (post) {
            return `<li>
                <h2>${post.title}</h2>
                <p>${post.body}</p>
            </li>`;
        });
        var html = htmls.join("");
        document.getElementById("post-block").innerHTML = html;
    })
    .catch(function (err) {
        console.log("Co loi!");
    });

/**
 * JS toggle
 *
 * Cách dùng:
 * <button class="js-toggle" toggle-target="#box">Click</button>
 * <div id="box">Content show/hide</div>
 */
window.addEventListener("template-loaded", initJsToggle);

function initJsToggle() {
    $$(".js-toggle").forEach((button) => {
        const target = button.getAttribute("toggle-target");
        if (!target) {
            document.body.innerText = `Cần thêm toggle-target cho: ${button.outerHTML}`;
        }
        button.onclick = (e) => {
            e.preventDefault();

            if (!$(target)) {
                return (document.body.innerText = `Không tìm thấy phần tử "${target}"`);
            }
            const isHidden = $(target).classList.contains("hide");

            requestAnimationFrame(() => {
                $(target).classList.toggle("hide", !isHidden);
                $(target).classList.toggle("show", isHidden);
            });
        };
        document.onclick = function (e) {
            if (!e.target.closest(target)) {
                const isHidden = $(target).classList.contains("hide");
                if (!isHidden) {
                    button.click();
                }
            }
        };
    });
}

window.addEventListener("template-loaded", () => {
    const links = $$(".js-dropdown-list > li > a");

    links.forEach((link) => {
        link.onclick = () => {
            if (window.innerWidth > 991) return;
            const item = link.closest("li");
            item.classList.toggle("navbar__item--active");
        };
    });
});

//Chat
document.getElementById("out-chat").onclick = function () {
    this.style.display = "none";
    document.getElementById("chat-block").style.display = "flex";
    document.getElementById("chat-box__message").style.display = "block";
    document.getElementById("chat-block__add").style.display = "flex";
    document.getElementById("chat-container").style.display = "none";
    document.getElementById("diagnostic").innerHTML = "";
};

var arr = [];
arr = document.getElementsByClassName("chat-block__item");
for (let index = 0; index < arr.length; index++) {
    const element = arr[index];
    element.onclick = function () {
        document.getElementById("out-chat").style.display = "block";
        document.getElementById("chat-block").style.display = "none";
        document.getElementById("chat-box__message").style.display = "none";
        document.getElementById("chat-container").style.display = "flex";
        document.getElementById("chat-block__add").style.display = "none";
    };
}

function chanDoan() {
    return `<p class="chat-box__text">
    Chẩn đoán bệnh sỏi thận (hay còn gọi là
    bệnh sỏi đường tiết niệu) thường bao gồm
    các bước sau đây:
</p>

<p class="chat-box__text">
    1.
    <strong>
        Hỏi bệnh sử và khám lâm sàng:
    </strong>
    Bác sĩ sẽ hỏi về triệu chứng, tiền sử
    bệnh lý, thói quen sinh hoạt và chế độ
    ăn uống của bệnh nhân. Một số triệu
    chứng điển hình của sỏi thận bao gồm đau
    lưng, đau bụng dưới, buồn nôn, nôn mửa,
    tiểu ra máu, và tiểu khó.
</p>

<p class="chat-box__text">
    2.
    <strong>Xét nghiệm nước tiểu:</strong>
    Để tìm kiếm dấu hiệu của máu, vi khuẩn,
    hoặc tinh thể sỏi trong nước tiểu. Một
    số xét nghiệm nước tiểu có thể phát hiện
    sự có mặt của chất hóa học có thể dẫn
    đến hình thành sỏi.
</p>

<p class="chat-box__text">
    3. <strong>Xét nghiệm máu:</strong> Để
    kiểm tra mức độ canxi, phốt pho, axit
    uric và chức năng thận. Điều này giúp
    xác định nguyên nhân gây ra sỏi thận và
    đánh giá chức năng thận hiện tại.
</p>

<p class="chat-box__text">
    4. <strong>Phân tích sỏi:</strong> Nếu
    bệnh nhân có sỏi được loại bỏ hoặc đi ra
    ngoài theo đường tiểu, bác sĩ có thể yêu
    cầu phân tích sỏi để xác định thành phần
    hóa học của sỏi, từ đó đưa ra phương án
    phòng ngừa tái phát hiệu quả hơn.
</p>

<p class="chat-box__text">
    5.<strong>
        Thăm dò chức năng thận:
    </strong>
    Các xét nghiệm như chụp X-quang niệu
    quản, bàng quang (IVP), hoặc siêu âm
    doppler có thể được sử dụng để đánh giá
    mức độ ảnh hưởng của sỏi đến chức năng
    thận và hệ tiết niệu.
</p>

<p class="chat-box__text">
    Việc chẩn đoán chính xác bệnh sỏi thận
    là rất quan trọng để đưa ra phương án
    điều trị thích hợp và phòng ngừa tái
    phát sỏi trong tương lai. Nếu bạn nghi
    ngờ mình mắc bệnh sỏi thận, hãy tham
    khảo ý kiến bác sĩ để được tư vấn và
    thực hiện các xét nghiệm cần thiết.
</p>`;
}
document.getElementById("chuanDoan").onclick = function () {
    document.getElementById("diagnostic").innerHTML = chanDoan();
};

function trieuChung() {
    return ` <p class="chat-box__text">
    Sỏi thận có thể gây ra nhiều triệu chứng
    khác nhau tùy thuộc vào vị trí, kích
    thước của sỏi và mức độ tắc nghẽn đường
    tiết niệu. Dưới đây là một số triệu
    chứng phổ biến của sỏi thận:
</p>

<p class="chat-box__text">
    1.
    <strong
        >Đau lưng hoặc đau bụng
        dưới:</strong
    >
    <br />
    + Đau có thể bắt đầu ở vùng lưng dưới
    hoặc bên hông và lan xuống bụng dưới và
    háng.<br />
    + Đau có thể diễn ra từng cơn, từ đau
    nhẹ đến rất dữ dội.
</p>

<p class="chat-box__text">
    2. <strong>Tiểu ra máu:</strong> Nước
    tiểu có thể có màu hồng, đỏ, hoặc nâu do
    có máu trong nước tiểu (đái máu).
</p>

<p class="chat-box__text">
    3. <strong>Tiểu buốt, tiểu khó:</strong>
    Cảm giác đau buốt khi đi tiểu, hoặc khó
    tiểu.
</p>

<p class="chat-box__text">
    4. <strong>Tiểu nhiều lần:</strong>
    Cảm giác muốn đi tiểu thường xuyên hơn
    bình thường.
</p>

<p class="chat-box__text">
    5. <strong>Buồn nôn và nôn mửa:</strong>
    Buồn nôn và nôn mửa có thể xảy ra do đau
    hoặc do tắc nghẽn đường tiết niệu.
</p>

<p class="chat-box__text">
    6. <strong>Sốt và ớn lạnh:</strong> Nếu
    sỏi gây nhiễm trùng, bệnh nhân có thể bị
    sốt, ớn lạnh, và các triệu chứng giống
    như cúm.
</p>

<p class="chat-box__text">
    7.
    <strong>Khó chịu và không yên:</strong>
    Đau do sỏi thận thường rất khó chịu và
    có thể khiến bệnh nhân cảm thấy bồn
    chồn, không yên.
</p>

<p class="chat-box__text">
    8.
    <strong>
        Nước tiểu đục hoặc có mùi hôi:
    </strong>
    Nước tiểu có thể trở nên đục hoặc có mùi
    hôi nếu có nhiễm trùng kèm theo.
</p>

<p class="chat-box__text">
    Trong một số trường hợp, sỏi thận có thể
    không gây ra bất kỳ triệu chứng nào và
    chỉ được phát hiện tình cờ qua các xét
    nghiệm hình ảnh học. Tuy nhiên, khi sỏi
    bắt đầu di chuyển trong đường tiết niệu
    hoặc gây tắc nghẽn, các triệu chứng sẽ
    trở nên rõ ràng hơn và có thể rất đau
    đớn.
</p>

<p class="chat-box__text">
    Nếu bạn có bất kỳ triệu chứng nào kể
    trên, đặc biệt là đau dữ dội hoặc tiểu
    ra máu, bạn nên tìm kiếm sự tư vấn từ
    bác sĩ để được chẩn đoán và điều trị kịp
    thời.
</p>`;
}
document.getElementById("trieuChung").onclick = function () {
    document.getElementById("diagnostic").innerHTML = trieuChung();
};

function cheDoAn() {
    return ` <div class="row g-3">
    <div class="col-6">
        <div
            class="chat-box__container-wrap"
        >
            1. Salad rau xanh với nước chanh
            và dầu ô liu
        </div>
    </div>
    <div class="col-6">
        <div
            class="chat-box__container-wrap"
        >
            2. Cá hồi nướng với rau cải
        </div>
    </div>
    <div class="col-6">
        <div
            class="chat-box__container-wrap"
        >
            3. Cà rốt xào me
        </div>
    </div>
    <div class="col-6">
        <div
            class="chat-box__container-wrap"
        >
            4. Chè bí đỏ
        </div>
    </div>
    <div class="col-6">
        <div
            class="chat-box__container-wrap"
        >
            5. Xương hầm củ cải trắng
        </div>
    </div>
    <div class="col-6">
        <div
            class="chat-box__container-wrap"
        >
            6. Trứng bắc thảo nấu canh cá rô
        </div>
    </div>
</div>`;
}
document.getElementById("cheDoAn").onclick = function () {
    document.getElementById("diagnostic").innerHTML = cheDoAn();
};

function cheDoTap() {
    return ` <div class="row g-3">
    <div class="col-6">
        <div
            class="chat-box__container-wrap"
        >
            1. Bài tập tim mạch (cardio)
        </div>
    </div>
    <div class="col-6">
        <div
            class="chat-box__container-wrap"
        >
            2. Bài tập linh hoạt và cân bằng
        </div>
    </div>
    <div class="col-6">
        <div
            class="chat-box__container-wrap"
        >
            3. Bài tập sức mạnh
        </div>
    </div>
</div>`;
}
document.getElementById("cheDoTap").onclick = function () {
    document.getElementById("diagnostic").innerHTML = cheDoTap();
};

function luuY() {
    return `<p class="chat-box__text">
    Lưu ý khi tập luyện
</p>

<p class="chat-box__text">
    1. <strong>Tăng dần mức độ:</strong> Bắt
    đầu với mức độ tập luyện nhẹ và tăng dần
    theo thời gian để tránh chấn thương.
</p>

<p class="chat-box__text">
    2. <strong>Nghe cơ thể bạn:</strong> Nếu
    cảm thấy đau hoặc không thoải mái, nên
    ngừng tập và nghỉ ngơi.
</p>

<p class="chat-box__text">
    3. <strong>Uống đủ nước:</strong> Uống
    nước trước, trong và sau khi tập luyện
    để tránh mất nước, đặc biệt là khi tập
    luyện ngoài trời hoặc trong môi trường
    nóng.
</p>

<p class="chat-box__text">
    4.
    <strong
        >Tránh tập luyện quá sức:</strong
    >
    Không nên tập luyện quá mức, vì căng
    thẳng cơ thể quá nhiều có thể làm tăng
    nguy cơ hình thành sỏi thận.
</p>`;
}
document.getElementById("luuY").onclick = function () {
    document.getElementById("diagnostic").innerHTML = luuY();
};

function datLich() {
    return `  <div class="row g-3">
    <div class="col-6">
        <div
            class="chat-box__container-wrap chat-box__container-wrap--other"
        >
            <h4 class="chat-box__title">
                Phòng khám KCL
            </h4>
            <p class="chat-box__time">
                <em> Giờ làm việc:</em>
                07:00 - 11:00 & 13:00 -
                17:00
            </p>
            <div class="chat-box__box">
                <p
                    class="chat-box__location"
                >
                    <em>Địa chỉ:</em> 123 Lê
                    Đình Lý - Đà Nẵng
                </p>
                <button
                    class="chat-box__btn"
                >
                    Đặt lịch ngay
                </button>
            </div>
        </div>
    </div>
    <div class="col-6">
        <div
            class="chat-box__container-wrap chat-box__container-wrap--other"
        >
            <h4 class="chat-box__title">
                Bệnh viên đa khoa Gia Đình
            </h4>
            <p class="chat-box__time">
                <em>Giờ làm việc:</em> 07:00
                - 11:00 & 13:00 - 17:00
            </p>
            <div class="chat-box__box">
                <p
                    class="chat-box__location"
                >
                    <em>Địa chỉ:</em> Nguyễn
                    Hữu Thọ - Đà Nẵng
                </p>
                <button
                    class="chat-box__btn"
                >
                    Đặt lịch ngay
                </button>
            </div>
        </div>
    </div>
    <div class="col-6">
        <div
            class="chat-box__container-wrap chat-box__container-wrap--other"
        >
            <h4 class="chat-box__title">
                Bệnh viên y học cổ truyền
            </h4>
            <p class="chat-box__time">
                <em>Giờ làm việc:</em> 07:00
                - 11:00 & 13:00 - 17:00
            </p>
            <div class="chat-box__box">
                <p
                    class="chat-box__location"
                >
                    <em>Địa chỉ:</em> Đinh
                    Gia Trinh - Đà Nẵng
                </p>
                <button
                    class="chat-box__btn"
                >
                    Đặt lịch ngay
                </button>
            </div>
        </div>
    </div>
</div>`;
}
document.getElementById("datLich").onclick = function () {
    document.getElementById("diagnostic").innerHTML = datLich();

    var chatBox = [];
    chatBox = document.getElementsByClassName("chat-box__btn");
    for (let index = 0; index < chatBox.length; index++) {
        const element = chatBox[index];
        element.onclick = function (e) {
            var textEven =
                e.target.parentElement.parentElement.querySelector(
                    ".chat-box__title"
                ).innerText;
            document.getElementById("modal-message").innerHTML += textEven;

            document.getElementById("modal-message").style.display = "block";
        };
    }
};

//Push Image
document.getElementById("pushImage").onclick = function () {
    let input = document.createElement("input");
    input.type = "file";
    input.accept = "image/*";
    input.onchange = (event) => {
        let files = Array.from(input.files);
        if (files.length > 0) {
            let file = files[0];
            let reader = new FileReader();
            reader.onload = function (e) {
                let img = document.createElement("img");
                img.src = e.target.result;
                let message = document.createElement("div");
                message.classList.add("message-image");
                message.appendChild(img);
                document.getElementById("chat").appendChild(message);
            };
            reader.readAsDataURL(file);
        }
    };
    input.click();
};

//Push microphone

document.getElementById("mic").onclick = function () {
    let output = document.getElementById("inputMessage");
    var SpeechRecognition = SpeechRecognition || webkitSpeechRecognition;
    let recognition = new SpeechRecognition();

    // Thiết lập ngôn ngữ nhận diện là tiếng Việt
    recognition.lang = "vi-VN";

    recognition.onstart = () => {
        console.log(
            "Voice recognition started. Try speaking into the microphone."
        );
    };

    recognition.onspeechend = () => {
        recognition.stop();
        console.log("Voice recognition stopped.");
    };

    recognition.onresult = (e) => {
        var transcript = e.results[0][0].transcript;
        console.log("Transcript: ", transcript);
        output.value = transcript;
    };

    recognition.onerror = (event) => {
        console.error("Speech recognition error detected: " + event.error);
    };

    recognition.start();
};

//Clear
document.getElementById("clear").onclick = function () {
    let chatContainer = document.getElementById("chat");
    chatContainer.innerHTML = "";
};

//Chat message
// function message(text) {
//     return `<p
//     id="messageUser"
//     class="chat-message__text"
// >${text}</p>`;
// }

// let i = [0];
// document.getElementById("send").onclick = function () {
//     let mess = document.getElementById("inputMessage").value;
//     document.getElementById("chat-box__wrap").style.display = "none";
//     document.getElementById("inputMessage").value = "";
//     document.getElementById("chat").innerHTML += message(mess);

//     document.getElementById("chat").innerHTML += messageAI(fakeAI[i]);
//     i++;

//     document.getElementById("dauBung").onclick = function () {
//         document.getElementById("chat-block").style.display = "flex";
//     };
// };

//Fake AI
// function messageAI(text) {
//     return `<p
//     id="messageAI"
//     class="chat-message__text chat-message__text--AI"
// >${text}</p>`;
// }

// let fakeAI = [
//     "Chào bạn, tôi có thể giúp gì cho bạn ?",
//     "Bạn có thể cho tôi biết bạn bị đau ở vị trí nào không ?",
//     'Dựa vào triệu chứng bạn đưa ra, chúng tôi đã phát hiện ra rằng bạn có thể bị mắc phải một số vấn đề liên quan đến đau bụng dưới. Chúng tôi khuyên bạn nên tham khảo ý kiến của bác sĩ để được kiểm tra và chẩn đoán chính xác. Ngoài ra, bạn cũng có thể tham khảo một số lời khuyên sau đây: "Rửa tay sạch trước khi ăn và chế biến thực phẩm". Chúc bạn sức khỏe!',
//     'Nhấn vào đây để xem mô tả <button id="dauBung" class="chat-message__btn">Nhấn vào đây</button>',
//     "Cảm ơn bạn, bạn có thể xem phần chuẩn đoán ở bên trái để biết rõ hơn bệnh của mình nhé",
// ];

//Modal
document.addEventListener("click", ({ target }) => {
    if (
        !target.closest(".chat-box__btn") &&
        document.getElementById("modal-message").style.display == "block"
    ) {
        document.getElementById("modal-message").style.display = "none";
    }
});

// Icon chat feedback
const listIcon = document.getElementById("feedback");
const icon = document.getElementsByClassName("chat-message__icon");

for (let i = 0; i < icon.length; i++) {
    icon[i].addEventListener("click", function () {
        let current = document.getElementsByClassName("active");
        current[0].className = current[0].className.replace(" active", "");
        this.className += " active";
    });
}
