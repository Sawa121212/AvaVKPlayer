#ifndef BROWSER_HPP
#define BROWSER_HPP
#include <QWebEngineView>
#include <QUrlQuery>
#include <QThread>
#include <iostream>
#include "tcp.hpp"


class browser : public QObject {
    Q_OBJECT

public:
    browser(std::string _url, int _port);

    void run_browser();
private:
    void url_changed(const QUrl &url);

    int port;
    std::string url;
    bool ready = false;
    tcp_helper *tcp;
};
#endif // BROWSER_HPP
