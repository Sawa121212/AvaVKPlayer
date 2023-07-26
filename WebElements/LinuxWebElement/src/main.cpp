#include <QApplication>
#include <QTcpSocket>
#include <iostream>
#include "browser.hpp"
using namespace std;

int main(int argc, char* argv[]) {
  if(argc != 3) {
    cerr << "usage url port\n";
    return -1;
  }

  int port = stoi(argv[2]);
  string url = argv[1];
  QApplication app(argc, argv);

  browser b(url, port);
  b.run_browser();

  return app.exec();
}
