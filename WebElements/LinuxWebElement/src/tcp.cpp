#include "tcp.hpp"

tcp_helper::tcp_helper(int _port) {
    port = _port;
    socket = new QTcpSocket();
}

bool tcp_helper::connect() {
    socket->connectToHost("127.0.0.1", port);
    return socket->waitForConnected();
}

QString tcp_helper::read() { return QString(socket->readAll()); }

bool tcp_helper::write(QString msg) {
  socket->write(msg.toStdString().c_str());
  return socket->waitForBytesWritten();
}
