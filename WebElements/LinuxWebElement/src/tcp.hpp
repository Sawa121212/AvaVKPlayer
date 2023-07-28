#ifndef TCP_HPP
#define TCP_HPP

#include <QObject>
#include <QTcpSocket>

class tcp_helper : public QObject {
    Q_OBJECT
public:
    tcp_helper(int _port);
    bool connect();
    bool write(QString data);
    QString read();
private:
    QTcpSocket *socket;
    int port;
};

#endif //TCP_HPP
