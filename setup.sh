#!/bin/bash
cp docker-compose_sample.yml docker-compose.yml
cp docker-compose.db_sample.yml docker-compose.db.yml
echo "Edite os arquivos docker-compose.yml e docker-compose.db.yml"
echo "conforme a necessidade. Depois execute:"
echo "  sudo docker-compose -f docker-compose.db.yml up"
echo ""
echo "e, em outra tela: sudo docker-compose up"
echo ""
