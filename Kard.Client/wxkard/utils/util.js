const formatTime = date => {
  const year = date.getFullYear()
  const month = date.getMonth() + 1
  const day = date.getDate()
  const hour = date.getHours()
  const minute = date.getMinutes()
  const second = date.getSeconds()

  return [year, month, day].map(formatNumber).join('/') + ' ' + [hour, minute, second].map(formatNumber).join(':')
}

const formatNumber = n => {
  n = n.toString()
  return n[1] ? n : '0' + n
}

 
 
function px2rpx(px, windowWidth) {
  return Math.round(px * 750 / windowWidth);
}

function rpx2px(rpx, windowWidth) {
  return Math.round(rpx / 750 * windowWidth);
}

module.exports = {
  formatTime: formatTime,
  px2rpx: px2rpx,
  rpx2px: rpx2px,
}
