// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/**
 * 處理購物車非同步操作的通用函式
 * @param {string} url - 後端 Action 的路徑 (例如: /Sales/UpdateCart)
 * @param {object} data - 傳送給後端的參數物件 (例如: { productId: 1, qty: 2 })
 * @param {string} [confirmMessage] - 選填，觸發操作前是否跳出確認視窗
 */
function cartPost(url, data, confirmMessage) {
    // 1. 檢查是否需要跳出確認提示（例如：清空購物車時）
    if (confirmMessage && !confirm(confirmMessage)) {
        return;
    }

    // 2. 將 JavaScript 物件轉換成 URL 編碼格式 (Form Data)，以便後端 Action 接收
    const formData = new URLSearchParams();
    for (const key in data) {
        if (data.hasOwnProperty(key)) {
            formData.append(key, data[key]);
        }
    }

    // 3. 使用 fetch 發送非同步 POST 請求
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData.toString()
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('伺服器回應錯誤');
            }
            return response.text(); // 後端回傳的是 PartialView (HTML 內容)
        })
        .then(htmlString => {
            // 4. 尋找畫面上的購物車容器，並將內容替換為最新的 HTML
            const cartPanel = document.getElementById('cart-panel');
            if (cartPanel) {
                cartPanel.innerHTML = htmlString;
            } else {
                // 防呆：如果找不到容器，則直接重新整理網頁
                window.location.reload();
            }
        })
        .catch(error => {
            console.error('購物車更新失敗:', error);
            alert('操作失敗，請重新整理網頁再試一次。');
        });
}