start=$(date +'%s')
function_name=$1
url_name="https://$function_name.azurewebsites.net/api/$2_httpstart"
#url_name="http://localhost:7071/api/TestEntities_HttpStart"
file_name=$1_$2.txt
echo $file_name
echo $url_name
echo "Number of Tasks "$2
rm $file_name
seq 1 $3  | xargs -n 1 -P 1000 curl -s "$url_name" >>$file_name 
wait
cp $file_name /mnt/c/test/.
rm $file_name
echo "It took $(($(date +'%s') - $start)) seconds"
